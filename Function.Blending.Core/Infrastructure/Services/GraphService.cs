using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Menu.DTOs;
using Function.Blending.Core.Application.Menu.DTOs.Graph;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Infrastructure.Services
{
    
    public class GraphService : IGraphService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GraphService> _logger;
        private readonly IMapper _mapper;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public GraphService(HttpClient httpClient, ILogger<GraphService> logger, IMapper mapper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GraphUserInfo> GetUserInfoAsync(string accessToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new ArgumentException("Access token cannot be null or empty", nameof(accessToken));
                }

                _logger.LogInformation("Obteniendo información del usuario desde Microsoft Graph");

               
                var request = new HttpRequestMessage(HttpMethod.Get, $"{GraphApiConstants.BASE_URL}/me");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                
                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al obtener información del usuario: {StatusCode} - {Error}", 
                        response.StatusCode, errorContent);
                    throw new HttpRequestException($"Error al obtener información del usuario: {response.StatusCode}");
                }

                var userInfo = await DeserializeResponseAsync<GraphUserResponse>(response);
                
                if (userInfo == null)
                {
                    _logger.LogWarning("Microsoft Graph devolvió una respuesta nula para información del usuario");
                    throw new InvalidOperationException("No se pudo obtener información del usuario");
                }

 
                var result = _mapper.Map<GraphUserInfo>(userInfo);
                _logger.LogDebug("Información del usuario obtenida correctamente: {UserId}", result.Id);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información del usuario desde Microsoft Graph");
                throw;
            }
        }

        public async Task<List<string>> GetUserGroupsAsync(string accessToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new ArgumentException("Access token cannot be null or empty", nameof(accessToken));
                }

                _logger.LogInformation("Obteniendo grupos del usuario desde Microsoft Graph");

          
                var request = new HttpRequestMessage(HttpMethod.Get, $"{GraphApiConstants.BASE_URL}/me/memberOf");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                
                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al obtener grupos del usuario: {StatusCode} - {Error}", 
                        response.StatusCode, errorContent);
                    throw new HttpRequestException($"Error al obtener grupos del usuario: {response.StatusCode}");
                }


                var groupsResponse = await DeserializeResponseAsync<GraphGroupsResponse>(response);
                
                var groups = ExtractGroupIdentifiers(groupsResponse);

                _logger.LogInformation("Encontrados {GroupCount} grupos para el usuario", groups.Count);
                return groups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener grupos del usuario desde Microsoft Graph");
                throw;
            }
        }

        
        private async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning("Received empty response content for type {Type}", typeof(T).Name);
                    return null;
                }

                return JsonSerializer.Deserialize<T>(content, JsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response to type {Type}", typeof(T).Name);
                return null;
            }
        }

       
        private List<string> ExtractGroupIdentifiers(GraphGroupsResponse? groupsResponse)
        {
            var groups = new List<string>();
            
            if (groupsResponse?.Value == null)
            {
                _logger.LogDebug("No groups found in Graph API response");
                return groups;
            }

            foreach (var group in groupsResponse.Value)
            {
            
                if (!string.IsNullOrEmpty(group.Id))
                {
                    groups.Add(group.Id);
                    _logger.LogDebug("Added group ID: {GroupId}", group.Id);
                }
                else if (!string.IsNullOrEmpty(group.DisplayName))
                {
     
                    groups.Add(group.DisplayName);
                    _logger.LogDebug("Added group DisplayName (no ID available): {GroupName}", group.DisplayName);
                }
                else
                {
                    _logger.LogWarning("Skipped group with neither ID nor DisplayName");
                }
            }

            return groups;
        }
    }
}

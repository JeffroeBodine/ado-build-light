using System.Net;
using System.Text;
using System.Text.Json;
using NUnit.Framework;
using FluentAssertions;
using Moq;
using Moq.Protected;
using ADOBuildLight.Services;
using ADOBuildLight.Models;
using ADOBuildLight.Interfaces;
using static ADOBuildLight.Models.AppConfiguration;

namespace ADOBuildLight.Tests.ServiceTests
{
    [TestFixture]
    public class PipelineServiceTests
    {
        private Mock<IAppConfiguration> _mockConfig;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private PipelineService _pipelineService;

        [SetUp]
        public void SetUp()
        {
            _mockConfig = new Mock<IAppConfiguration>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            
            // Setup mock configuration with real objects
            var azureDevOpsSettings = new AzureDevOpsSettings
            {
                Organization = "test-org",
                Project = "test-project", 
                PipelineId = "123",
                PersonalAccessToken = "test-token"
            };
            
            _mockConfig.Setup(x => x.AzureDevOps).Returns(azureDevOpsSettings);

            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _pipelineService = new PipelineService(_mockConfig.Object, _httpClient);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public async Task GetLatestPipelineRunAsync_WithValidResponse_ReturnsLatestBuild()
        {
            // Arrange
            var pipelineRunsResponse = new PipelineRunsResponse
            {
                Count = 2,
                Value = new List<PipelineRun>
                {
                    new PipelineRun { Id = 1, CreatedDate = DateTime.Now.AddDays(-2), State = "completed", Result = "succeeded" },
                    new PipelineRun { Id = 2, CreatedDate = DateTime.Now.AddDays(-1), State = "completed", Result = "failed" }
                }
            };

            var buildResponse = new BuildResponse
            {
                Status = "completed",
                Result = "failed"
            };

            var pipelineRunsJson = JsonSerializer.Serialize(pipelineRunsResponse);
            var buildResponseJson = JsonSerializer.Serialize(buildResponse);

            _mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(pipelineRunsJson, Encoding.UTF8, "application/json")
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(buildResponseJson, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _pipelineService.GetLatestPipelineRunAsync();

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("completed");
            result.Result.Should().Be("failed");
        }

        [Test]
        public async Task GetLatestPipelineRunAsync_WithEmptyPipelineRuns_ReturnsNull()
        {
            // Arrange
            var pipelineRunsResponse = new PipelineRunsResponse
            {
                Count = 0,
                Value = new List<PipelineRun>()
            };

            var pipelineRunsJson = JsonSerializer.Serialize(pipelineRunsResponse);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(pipelineRunsJson, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _pipelineService.GetLatestPipelineRunAsync();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetLatestPipelineRunAsync_WithHttpException_ReturnsNull()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _pipelineService.GetLatestPipelineRunAsync();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetLatestPipelineRunAsync_WithInvalidJson_ReturnsNull()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("invalid json", Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _pipelineService.GetLatestPipelineRunAsync();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetLatestPipelineRunAsync_WithNullPipelineRunsResponse_ReturnsNull()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("null", Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _pipelineService.GetLatestPipelineRunAsync();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetLatestPipelineRunAsync_BuildDetailsHttpException_ReturnsNull()
        {
            // Arrange
            var pipelineRunsResponse = new PipelineRunsResponse
            {
                Count = 1,
                Value = new List<PipelineRun>
                {
                    new PipelineRun { Id = 1, CreatedDate = DateTime.Now, State = "completed", Result = "succeeded" }
                }
            };

            var pipelineRunsJson = JsonSerializer.Serialize(pipelineRunsResponse);

            _mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(pipelineRunsJson, Encoding.UTF8, "application/json")
                })
                .ThrowsAsync(new HttpRequestException("Build details error"));

            // Act
            var result = await _pipelineService.GetLatestPipelineRunAsync();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetLatestPipelineRunAsync_BuildDetailsInvalidJson_ReturnsNull()
        {
            // Arrange
            var pipelineRunsResponse = new PipelineRunsResponse
            {
                Count = 1,
                Value = new List<PipelineRun>
                {
                    new PipelineRun { Id = 1, CreatedDate = DateTime.Now, State = "completed", Result = "succeeded" }
                }
            };

            var pipelineRunsJson = JsonSerializer.Serialize(pipelineRunsResponse);

            _mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(pipelineRunsJson, Encoding.UTF8, "application/json")
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("invalid build json", Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _pipelineService.GetLatestPipelineRunAsync();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetLatestPipelineRunAsync_OrdersByCreatedDateDescending()
        {
            // Arrange
            var olderDate = DateTime.Now.AddDays(-3);
            var newerDate = DateTime.Now.AddDays(-1);
            
            var pipelineRunsResponse = new PipelineRunsResponse
            {
                Count = 2,
                Value = new List<PipelineRun>
                {
                    new PipelineRun { Id = 1, CreatedDate = olderDate, State = "completed", Result = "succeeded" },
                    new PipelineRun { Id = 2, CreatedDate = newerDate, State = "completed", Result = "failed" }
                }
            };

            var buildResponse = new BuildResponse
            {
                Status = "completed",
                Result = "failed"
            };

            var pipelineRunsJson = JsonSerializer.Serialize(pipelineRunsResponse);
            var buildResponseJson = JsonSerializer.Serialize(buildResponse);

            _mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(pipelineRunsJson, Encoding.UTF8, "application/json")
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(buildResponseJson, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _pipelineService.GetLatestPipelineRunAsync();

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().Be("failed"); // Should get the newer build (ID 2) result
        }

        [Test]
        public async Task GetLatestPipelineRunAsync_SetsCorrectHeaders()
        {
            // Arrange
            var pipelineRunsResponse = new PipelineRunsResponse
            {
                Count = 1,
                Value = new List<PipelineRun>
                {
                    new PipelineRun { Id = 1, CreatedDate = DateTime.Now, State = "completed", Result = "succeeded" }
                }
            };

            var buildResponse = new BuildResponse { Status = "completed", Result = "succeeded" };
            var pipelineRunsJson = JsonSerializer.Serialize(pipelineRunsResponse);
            var buildResponseJson = JsonSerializer.Serialize(buildResponse);

            var requestCount = 0;
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    requestCount++;
                    
                    // Verify headers for both requests
                    request.Headers.Accept.Should().Contain(h => h.MediaType == "application/json");
                    request.Headers.Authorization.Should().NotBeNull();
                    request.Headers.Authorization.Scheme.Should().Be("Basic");
                    
                    if (requestCount == 1)
                    {
                        // First request - pipeline runs
                        request.RequestUri?.ToString().Should().Contain("_apis/pipelines");
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(pipelineRunsJson, Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        // Second request - build details
                        request.RequestUri?.ToString().Should().Contain("_apis/build/builds");
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(buildResponseJson, Encoding.UTF8, "application/json")
                        };
                    }
                });

            // Act
            var result = await _pipelineService.GetLatestPipelineRunAsync();

            // Assert
            result.Should().NotBeNull();
            requestCount.Should().Be(2);
        }

        [Test]
        public void Constructor_InitializesCorrectly()
        {
            // Act & Assert
            _pipelineService.Should().NotBeNull();
        }
    }
}
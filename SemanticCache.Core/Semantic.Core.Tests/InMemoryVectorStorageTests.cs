using Xunit;
using Semantic.Core;
using Microsoft.Extensions.Options;

namespace Semantic.Core.Tests;

public class inMemoryVectorStorageTests
{
    [Fact]
    public async Task SaveAsync_WhenLimitExceeded_EvictsOldestItem()
    {
        SemanticCacheOptions semanticCacheOptions = new SemanticCacheOptions
        {
            MaxItems = 2
        };
        InMemoryVectorStorage inMemoryVectorStorage = new InMemoryVectorStorage(Options.Create(semanticCacheOptions));

        float[] floatArray1 = {1, 0, 0};
        float[] floatArray2 = {0, 1, 0};
        float[] floatArray3 = {0, 0, 1};

        await inMemoryVectorStorage.SaveAsync("req1", floatArray1, "res1");
        await inMemoryVectorStorage.SaveAsync("req2", floatArray2, "res2");
        await inMemoryVectorStorage.SaveAsync("req3", floatArray3, "res3");

        string searchResult = await inMemoryVectorStorage.FindSimilarAsync(floatArray1, 0.85f, true);
        Assert.Equal(2, inMemoryVectorStorage.Count);

        Assert.Null(searchResult);
    }

    [Fact]
public async Task FindSimilarAsync_WhenSimilarityAboveThreshold_ReturnsResponse()
{

    SemanticCacheOptions semanticCacheOptions = new SemanticCacheOptions();
    InMemoryVectorStorage inMemoryVectorStorage = new InMemoryVectorStorage(Options.Create(semanticCacheOptions));

    float[] floatArray1 = {1, 0, 0};
    
    await inMemoryVectorStorage.SaveAsync("req1", floatArray1, "res1");

    string searchResult = await inMemoryVectorStorage.FindSimilarAsync(floatArray1, 0.90f, true);

    Assert.Equal("res1", searchResult);
}

[Fact]
public async Task FindSimilarAsync_WhenSimilarityBelowThreshold_ReturnsNull()
{

    SemanticCacheOptions semanticCacheOptions = new SemanticCacheOptions();
    InMemoryVectorStorage inMemoryVectorStorage = new InMemoryVectorStorage(Options.Create(semanticCacheOptions));

    float[] floatArray1 = {1, 0, 0};
    float[] floatArray2 = {0, 1, 0};
    
    await inMemoryVectorStorage.SaveAsync("req1", floatArray1, "res1");

    string searchResult = await inMemoryVectorStorage.FindSimilarAsync(floatArray2, 0.50f, true);

    Assert.Null(searchResult);
}

[Fact]
public async Task FindSimilarAsync_WhenItemFound_PromotesToRecentlyUsed()
{
    SemanticCacheOptions semanticCacheOptions = new SemanticCacheOptions
        {
            MaxItems = 2
        };

    InMemoryVectorStorage inMemoryVectorStorage = new InMemoryVectorStorage(Options.Create(semanticCacheOptions));

    float[] floatArray1 = {1, 0, 0};
    float[] floatArray2 = {0, 1, 0};
    float[] floatArray3 = {0, 0, 1};

    await inMemoryVectorStorage.SaveAsync("req1", floatArray1, "res1");
    await inMemoryVectorStorage.SaveAsync("req2", floatArray2, "res2");

    await inMemoryVectorStorage.FindSimilarAsync(floatArray1, 0.90f, true);

    await inMemoryVectorStorage.SaveAsync("req3", floatArray3, "res3");

    string searchVectorTwo = await inMemoryVectorStorage.FindSimilarAsync(floatArray2, 0.90f, true);
    string searchVectorOne = await inMemoryVectorStorage.FindSimilarAsync(floatArray1, 0.90f, true);

    Assert.Null(searchVectorTwo);
    Assert.Equal("res1", searchVectorOne);
}
[Fact]
public async Task Cache_UnderConcurrentReadWrite_DoesNotThrowAndMaintainsLimit()
{
    SemanticCacheOptions semanticCacheOptions = new SemanticCacheOptions
    {
        MaxItems = 50
    };
    InMemoryVectorStorage inMemoryVectorStorage = new InMemoryVectorStorage(Options.Create(semanticCacheOptions));

    float[] floatArray1 = {1, 0, 0};
    List<Task> tasks = new();

    for(int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(async () => await inMemoryVectorStorage.SaveAsync("req1", floatArray1, "res1")));
            tasks.Add(Task.Run(async () => await inMemoryVectorStorage.FindSimilarAsync(floatArray1, 0.90f, true)));
        }

    await Task.WhenAll(tasks);
    Assert.True(inMemoryVectorStorage.Count <= 50);
}
[Fact]
public async Task FindSimilarAsync_WhenCacheIsEmpty_ReturnsNull()
{
    SemanticCacheOptions semanticCacheOptions = new SemanticCacheOptions();

    InMemoryVectorStorage inMemoryVectorStorage = new InMemoryVectorStorage(Options.Create(semanticCacheOptions));

    float[] floatArray1 = {1, 0, 0};
    string searchResult = await inMemoryVectorStorage.FindSimilarAsync(floatArray1, 0.90f, true);

    Assert.Null(searchResult);
}

[Fact]
public async Task SaveAsync_WhenVectorIsNull_ThrowsArgumentNullException()
{
    SemanticCacheOptions semanticCacheOptions = new SemanticCacheOptions();

    InMemoryVectorStorage inMemoryVectorStorage = new InMemoryVectorStorage(Options.Create(semanticCacheOptions));

    await Assert.ThrowsAsync<ArgumentNullException>(async () => await inMemoryVectorStorage.SaveAsync("req1", null, "res1"));
}

[Fact]
public async Task SaveAsync_WhenVectorIsEmpty_ThrowsArgumentException()
{
    SemanticCacheOptions semanticCacheOptions = new SemanticCacheOptions();

    InMemoryVectorStorage inMemoryVectorStorage = new InMemoryVectorStorage(Options.Create(semanticCacheOptions));

    float[] emptyVector = Array.Empty<float>();
    await Assert.ThrowsAsync<ArgumentException>(async () => await inMemoryVectorStorage.SaveAsync("req1", emptyVector, "res1"));
}

[Fact]
public async Task Constructor_WhenMaxItemsIsInvalid_ThrowsArgumentOutOfRangeException()
{
    SemanticCacheOptions semanticCacheOptions = new SemanticCacheOptions
    {
        MaxItems = -1
    };

    Assert.Throws<ArgumentOutOfRangeException>(() => new InMemoryVectorStorage(Options.Create(semanticCacheOptions)));
}
}
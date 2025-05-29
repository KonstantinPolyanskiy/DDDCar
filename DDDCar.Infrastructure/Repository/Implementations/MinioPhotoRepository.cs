using DDDCar.Application.Repositories;
using DDDCar.Core.DtoObjects;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace DDDCar.Infrastructure.Repository.Implementations;

public class MinioPhotoRepository(IMinioClient client, ILogger<MinioPhotoRepository> log) : IPhotoRepository
{
    private const string Bucket = "PhotoBucket";
    
    public async Task<bool> AddAsync(PhotoDto photo)
    {
        try
        {
            await CheckAndCreateBucket(Bucket);
            
            await using var ms = new MemoryStream(photo.Data);
            
            var args = new PutObjectArgs()
                .WithBucket(Bucket)
                .WithObject(photo.EntityId.ToString())
                .WithStreamData(ms)
                .WithObjectSize(ms.Length)
                .WithContentType("image/" + photo.Extension);

            await client.PutObjectAsync(args);
            
            log.LogInformation("Фото {id} сохранено в БД", photo.EntityId);
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid photoId)
    {
        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(Bucket)
                .WithObject(photoId.ToString());
            
            await client.RemoveObjectAsync(args);
            
            log.LogInformation("Фото {id} удалено из БД", photoId);
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<PhotoDto?> FindByIdAsync(Guid photoId)
    {
        try
        {
            var stat = await client.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(Bucket)
                    .WithObject(photoId.ToString()));

            stat.MetaData.TryGetValue("x-amz-meta-ext", out var ext);
            var extension = !string.IsNullOrWhiteSpace(ext)
                ? ext
                : stat.ContentType?.Split('/').Last() ?? "bin";

            await using var ms = new MemoryStream();
            await client.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(Bucket)
                    .WithObject(photoId.ToString())
                    .WithCallbackStream(async s => await s.CopyToAsync(ms)));

            return new PhotoDto
            {
                EntityId = photoId,
                Extension = extension,
                Data = ms.ToArray()
            };
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }
    
    private async Task CheckAndCreateBucket(string bucketName)
    {
        var exist = await client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (exist is false)
            await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    }
}
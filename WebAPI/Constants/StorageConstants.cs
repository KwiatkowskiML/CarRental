namespace WebAPI.Constants;

public static class StorageConstants
{
    public static string BucketName { get; private set; } = "car-images-dev-0";
    public static string PublicUrlPrefix = "https://storage.googleapis.com";

    public static void Initialize(string bucketName, string publicUrlPrefix)
    {
        BucketName = bucketName;
        PublicUrlPrefix = publicUrlPrefix;
    }
}
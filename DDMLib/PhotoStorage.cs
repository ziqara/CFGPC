using System;
using System.IO;

public static class PhotoStorage
{
    private const string DbPrefix = "/Resources/";

    public static string SavePhotoToResources(string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
            return null;

        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Файл изображения не найден.", sourcePath);

        string targetFolder = FindWebResourcesFolder();

        if (targetFolder == null)
            throw new Exception("Не найдена папка WebApplication1/wwwroot/Resources");

        Directory.CreateDirectory(targetFolder);

        string extension = Path.GetExtension(sourcePath);
        string newFileName = Guid.NewGuid().ToString("N") + extension;

        string fullTargetPath = Path.Combine(targetFolder, newFileName);

        File.Copy(sourcePath, fullTargetPath, true);

        // В БД будет /Resources/xxxx.jpg
        return DbPrefix + newFileName;
    }

    private static string FindWebResourcesFolder()
    {
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        for (int i = 0; i < 10 && dir != null; i++)
        {
            string candidate = Path.Combine(dir.FullName, "WebApplication1", "wwwroot", "Resources");
            if (Directory.Exists(candidate))
                return candidate;

            dir = dir.Parent;
        }

        return null;
    }
}
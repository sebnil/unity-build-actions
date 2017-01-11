﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace SuperSystems.UnityBuild
{

public class FolderOperation : BuildAction, IPreBuildAction, IPreBuildPerPlatformAction, IPostBuildAction, IPostBuildPerPlatformAction
{
    public enum Operation
    {
        Move,
        Copy,
        Delete,
    }

    [FilePath(true)]
    public string inputPath;
    [FilePath(true)]
    public string outputPath;
    public Operation operation;

    public override void Execute()
    {
        switch (operation)
        {
            case Operation.Copy:
                Copy(inputPath, outputPath);
                break;
            case Operation.Move:
                Move(inputPath, outputPath);
                break;
            case Operation.Delete:
                Delete(inputPath);
                break;
        }
    }

    public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
    {
        string resolvedInputPath = BuildProject.ResolvePath(inputPath.Replace("$BUILDPATH", buildPath), releaseType, platform, architecture, distribution, buildTime);
        string resolvedOutputPath = BuildProject.ResolvePath(outputPath.Replace("$BUILDPATH", buildPath), releaseType, platform, architecture, distribution, buildTime);

        switch (operation)
        {
            case Operation.Copy:
                Copy(resolvedInputPath, resolvedOutputPath);
                break;
            case Operation.Move:
                Move(resolvedInputPath, resolvedOutputPath);
                break;
            case Operation.Delete:
                Delete(resolvedInputPath);
                break;
        }
    }

    protected override void DrawProperties(SerializedObject obj)
    {
        EditorGUILayout.PropertyField(obj.FindProperty("operation"));
        EditorGUILayout.PropertyField(obj.FindProperty("inputPath"));

        if (operation != Operation.Delete)
            EditorGUILayout.PropertyField(obj.FindProperty("outputPath"));
    }

    private void Move(string inputPath, string outputPath, bool overwrite = true)
    {
        if (!Directory.Exists(inputPath))
        {
            // Error. Input does not exist.
            return;
        }

        if (overwrite && Directory.Exists(outputPath))
        {
            // Delete previous output.
            FileUtil.DeleteFileOrDirectory(outputPath);
        }

        FileUtil.MoveFileOrDirectory(inputPath, outputPath);
    }

    private void Copy(string inputPath, string outputPath, bool overwrite = true)
    {
        if (!Directory.Exists(inputPath))
        {
            // Error. Input does not exist.
            return;
        }

        if (overwrite && Directory.Exists(outputPath))
        {
            // Delete previous output.
            FileUtil.DeleteFileOrDirectory(outputPath);
        }

        FileUtil.CopyFileOrDirectory(inputPath, outputPath);
    }

    private void Delete(string inputPath)
    {
        if (Directory.Exists(inputPath))
        {
            FileUtil.DeleteFileOrDirectory(inputPath);
        }
        else
        {
            // Error. File does not exist.
        }
    }
}

}
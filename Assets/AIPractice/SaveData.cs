using System;
using CollectItem.Stage;
using UnityEngine;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;

namespace CollectItem.Data
{
  public static class SaveData
  {
      /// <summary>
      /// ステージデータを上書き保存する
      /// </summary>
      /// <param name="stageDataSO"></param>
      /// <param name="stateData"></param>
      public static void SaveStageData(StageDataSO stageDataSO, StageData stateData)
      { 
        SaveStageData(stageDataSO.stageDataJson, stateData);
      }

      /// <summary>
      /// ステージデータを上書き保存する
      /// </summary>
      /// <param name="jsonFile"></param>
      /// <param name="stageData"></param>
      public static void SaveStageData(TextAsset jsonFile, StageData stageData)
      {
          //textAssetの場所を取得
          string path = AssetDatabase.GetAssetPath(jsonFile);
          string jsonStr = JsonConvert.SerializeObject(stageData, Formatting.Indented);
          SaveJsonData(path,jsonStr);
      }
      
      /// <summary>
      /// Jsonデータを保存する
      /// </summary>
      /// <param name="path"></param>
      /// <param name="jsonStr"></param>
      private static void SaveJsonData(string path, string jsonStr)
      {
          using (StreamWriter writer = new StreamWriter(path, false))
          {
              writer.Write(jsonStr);
          }
          AssetDatabase.Refresh();
      }

      /// <summary>
      /// 新しくステージデータを作成する
      /// </summary>
      /// <param name="savePath">保存先</param>
      /// <param name="stageData">保存するステージデータ</param>
      public static void CreateNewStageJsonFile(DefaultAsset savePath, StageData stageData)
      {
          CreateNewStageJsonFile(savePath, stageData, out TextAsset jsonFile);
      }

      /// <summary>
      /// 新しくステージデータを作成する
      /// </summary>
      /// <param name="savePath">保存先</param>
      /// <param name="stageData">保存するステージデータ</param>
      /// <param name="jsonFile">保存したJsonFile</param>
      public static void CreateNewStageJsonFile(DefaultAsset savePath, StageData stageData, out TextAsset jsonFile)
      {
          string filePath = GetNewFileName(savePath, "NewStageData", ".json");
          string jsonStr = JsonConvert.SerializeObject(stageData, Formatting.Indented);
          using(StreamWriter writer = new StreamWriter(filePath, false))
          {
              writer.Write(jsonStr);
          }
          
          AssetDatabase.Refresh();
          jsonFile = GetJsonData(filePath);
      }

      /// <summary>
      /// 新しくステージデータSOを作成する
      /// </summary>
      /// <param name="savePath">保存するパス</param>
      /// <param name="stageDataSOName">新しいSOの名前</param>
      /// <param name="stageData">保存するステージデータ</param>
      public static void CreateNewStageDataSO(DefaultAsset savePath, string stageDataSOName, TextAsset stageData)
      {
          string filePath = GetNewFileName(savePath, stageDataSOName, ".asset");
          
          //新しくStageDataSOをfilePathの位置に作成
          var stageDataSO = ScriptableObject.CreateInstance<StageDataSO>();
          stageDataSO.stageDataJson = stageData;
            AssetDatabase.CreateAsset(stageDataSO, filePath);
      }

      /// <summary>
      /// ファイル名を取得する
      /// </summary>
      /// <param name="savePath"></param>
      /// <param name="baseFileName"></param>
      /// <param name="taleStr"></param>
      /// <param name="id"></param>
      /// <returns></returns>
      private static string GetNewFileName(DefaultAsset savePath, string baseFileName, string taleStr,int id = 0)
      {
          string path = AssetDatabase.GetAssetPath(savePath) + "/";
          // ファイル名を作成
          string fileName = (id == 0) ? baseFileName + taleStr : baseFileName + id.ToString() + taleStr;
          string filePath = Path.Combine(path, fileName);

          if (!File.Exists(filePath))
          {
              return filePath;
          }
            
          return GetNewFileName(savePath, baseFileName, taleStr, id + 1);
      }
      
      /// <summary>
      /// Jsonデータを読み込む
      /// </summary>
      /// <param name="textAsset"></param>
      /// <typeparam name="T"></typeparam>
      /// <returns></returns>
      /// <exception cref="ArgumentException"></exception>
      public static T LoadData<T>(TextAsset textAsset)
      {
          if (textAsset == null) throw new ArgumentException(nameof(textAsset));

          try
          {
              T data = JsonConvert.DeserializeObject<T>(textAsset.text);
              return data;
          }
          catch (JsonException e)
          {
              Debug.LogError($"Jsonのデシリアライズに失敗： {e.Message}");
              throw;
          }
      }
      
      public static TextAsset GetJsonData(string path)
      {
          return (TextAsset)AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
      }
  }
}
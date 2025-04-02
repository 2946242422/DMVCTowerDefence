//UserInfo.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Realms;
using UnityEngine; // ����Linq�������������

public class UserInfo : RealmObject
{
    [PrimaryKey] public string UserId { get; set; }

    /// <summary>
    /// �û���
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// �û��ȼ�
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// ��ǰ����ֵ
    /// </summary>
    public int CurrentExperience { get; set; }

    /// <summary>
    /// �������辭��ֵ
    /// </summary>
    public int MaxExperience { get; set; }

    /// <summary>
    /// ����ֵ
    /// </summary>
    public int CurrentEnergy { get; set; }


    /// <summary>
    /// �������
    /// </summary>
    public int MaxEnergy { get; set; }

    /// <summary>
    /// ��ʯ����
    /// </summary>
    public int Gems { get; set; }

    /// <summary>
    /// �������
    /// </summary>
    public int Coins { get; set; }

    /// <summary>
    /// ������
    /// </summary>
    public int ExtraCoins { get; set; }

    /// <summary>
    ///  �û�����ɵ��½ں͹ؿ���Ϣ
    /// </summary>
    public IList<ChapterProgress> ChapterProgresses { get;  }

    /// <summary>
    /// ��ǰ��������
    /// </summary>
    public int CurrentStars { get; set; }

    /// <summary>
    /// �����������
    /// </summary>
    public int MaxStars { get; set; }

    public string PasswordHash { get; set; } // �洢����Ĺ�ϣֵ

    public UserInfo()
    {
        UserId = Guid.NewGuid().ToString();
        // ����Ĭ��ֵ
        Level = 1;
        CurrentExperience = 0;
        MaxExperience = 100;
        CurrentEnergy = 50;
        MaxEnergy = 50;
        Gems = 0;
        Coins = 100;
        ExtraCoins = 0;
        CurrentStars = 0;
        MaxStars = 0; // ����������Ϸ�������
    }
    // UserInfo.cs
    private string ComputeHash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            string hashString = Convert.ToBase64String(hashBytes); // ����ϣֵת��Ϊ�ַ����Ա���־���
            Debug.Log($"ComputeHash: ��������: {input}, ��ϣֵ: {hashString}"); // ������־
            return hashString;
        }
    }

    public void SetPassword(string password)
    {
        PasswordHash = ComputeHash(password);
        Debug.Log($"SetPassword: ���������ϣΪ: {PasswordHash}"); // ������־
    }

    public bool VerifyPassword(string password)
    {
        string inputHash = ComputeHash(password);
        bool isCorrect = PasswordHash == inputHash;
        Debug.Log($"VerifyPassword: ���������ϣ: {inputHash}, �洢�����ϣ: {PasswordHash}, ��֤���: {isCorrect}"); // ������־
        return isCorrect;
    }

    /// <summary>
    /// ��ȡָ���½ڵĽ��ȡ�����½ڲ����ڣ��򴴽�һ���µġ�
    /// </summary>
    /// <param name="chapterNumber">�½ڱ��</param>
    /// <returns>�½ڽ���</returns>
    public void GetOrCreateChapterProgress(int chapterNumber) //��Ϊvoid
    {
        Realm realm = Realm.GetInstance(); // �������ڲ���ȡ Realm ʵ��
        realm.Write(() =>
        {
             ChapterProgress progress = ChapterProgresses.FirstOrDefault(cp => cp.ChapterNumber == chapterNumber);
            if (progress == null)
            {
                progress = new ChapterProgress(chapterNumber);
                realm.Add(progress); // ���ӵ� Realm
            }
        });
    }

    /// <summary>
    /// ��ȡָ���½ں͹ؿ����Ǽ���
    /// </summary>
    /// <param name="chapterNumber">�½ڱ��</param>
    /// <param name="stageNumber">�ؿ����</param>
    /// <returns>�Ǽ���0��ʾδ������δ��ɣ�</returns>
    public int GetStageStars(int chapterNumber, int stageNumber)
    {
        ChapterProgress chapterProgress = ChapterProgresses.FirstOrDefault(cp => cp.ChapterNumber == chapterNumber);
        if (chapterProgress != null)
        {
            return chapterProgress.GetStageStars(stageNumber);
        }

        return 0; // �½ڲ����ڣ�����0
    }

    /// <summary>
    ///  �����ؿ��������Ǽ�
    /// </summary>
    /// <param name="chapterNumber"></param>
    /// <param name="stageNumber"></param>
    /// <param name="stars"></param>
    public void UnlockAndSetStageStars(int chapterNumber, int stageNumber, int stars)
    {
       // ChapterProgress chapterProgress = GetOrCreateChapterProgress(chapterNumber); //����Ҫ����ֵ��
       // chapterProgress.UnlockAndSetStageStars(stageNumber, stars);
        GetOrCreateChapterProgress(chapterNumber);
        Realm realm = Realm.GetInstance();
        realm.Write(()=>
        {
            ChapterProgress chapterProgress =  ChapterProgresses.FirstOrDefault(cp => cp.ChapterNumber == chapterNumber);
            chapterProgress.UnlockAndSetStageStars(stageNumber,stars);
        });
        UpdateCurrentStars(); //��������
    }

    /// <summary>
    /// ���µ�ǰ��������
    /// </summary>
    public void UpdateCurrentStars()
    {
        //CurrentStars = ChapterProgresses.ToList().Sum(cp => cp.Stages.ToList().Sum(s => s.Stars)); // ���Ƽ�
         Realm realm = Realm.GetInstance();
         realm.Write(()=>{
            CurrentStars = ChapterProgresses.Sum(cp => cp.Stages.Sum(s => s.Stars));
         });
    }
}

/// <summary>
/// �½ڽ�����
/// </summary>
public class ChapterProgress : RealmObject
{
    [PrimaryKey] public string Id { get; set; }

    /// <summary>
    /// �½ڱ��
    /// </summary>
    public int ChapterNumber { get; set; }

    /// <summary>
    /// �½��ڵĹؿ������б�.  ��List�����.
    /// </summary>
    public IList<StageProgress> Stages { get; } // ����ʹ�� IList<T>

    public ChapterProgress() // �����޲ι��캯��
   {
       Id = Guid.NewGuid().ToString();
       Stages = new List<StageProgress>(); // ��ʼ�� Stages
   }

    public ChapterProgress(int chapterNumber)
    {
        ChapterNumber = chapterNumber;
        Id = System.Guid.NewGuid().ToString(); //���� $"{chapterNumber}_{System.Guid.NewGuid()}" ������ʽ.
        Stages = new List<StageProgress>(); //������Գ�ʼ��
    }

    /// <summary>
    /// ��ȡ�ؿ���������
    /// </summary>
    /// <param name="stageNumber"></param>
    /// <returns></returns>
    public int GetStageStars(int stageNumber)
    {
        StageProgress stageProgress = Stages.FirstOrDefault(sp => sp.StageNumber == stageNumber);
        return stageProgress != null ? stageProgress.Stars : 0;
    }

    /// <summary>
    /// �����ؿ���������������
    /// </summary>
    /// <param name="stageNumber"></param>
    /// <param name="stars"></param>
     public void UnlockAndSetStageStars(int stageNumber, int stars)
    {
        StageProgress stage = Stages.FirstOrDefault(s => s.StageNumber == stageNumber);
        if (stage == null)
        {
            stage = new StageProgress { StageNumber = stageNumber, Stars = stars,Id = Guid.NewGuid().ToString() };
            // Stages.Add(stage);  // �Ƴ���һ�У�
            Realm realm = Realm.GetInstance(); // ��ȡ Realm ʵ��, �����ӵ�������
            realm.Write(()=>
            {
               realm.Add(stage);
            });
        }
        else
        {
            // Realm ���Զ����ٶ� stage.Stars ���޸�
            Realm realm = Realm.GetInstance();
            realm.Write(() =>
            {
                stage.Stars = System.Math.Max(stage.Stars, stars);
                if (stage.Stars > 3) stage.Stars = 3;
            });
        }
    }
}

/// <summary>
/// �ؿ�������
/// </summary>
public class StageProgress : RealmObject
{
    [PrimaryKey] public string Id { get; set; }

    /// <summary>
    /// �ؿ����
    /// </summary>
    public int StageNumber { get; set; }

    /// <summary>
    ///  ��õ��Ǽ� (0 ��ʾδ��ɻ�δ����, 1-3 ��ʾ��õ��Ǽ�).
    /// </summary>
    public int Stars { get; set; }
    public StageProgress()
    {
        Id = System.Guid.NewGuid().ToString();
    }
}

using System.Collections.Generic;
using UnityEngine;

public class RealtimeMsg : MonoBehaviour
{
    [SerializeField]
    private Transform MessagePanel;
    [SerializeField]
    private SingleMessage singleMessage;

    private List<SingleMessage> allMessages;
    private List<string> randomMessages;

    void Start()
    {
        allMessages = new List<SingleMessage>();
        InitRandomTipsMessage();
    }

    private void InitRandomTipsMessage()
    {
        randomMessages = new List<string>();
        randomMessages.Add("�Ҽ���������е��ƿɷ�������");
        randomMessages.Add("�Ҽ�˫�������е����������鱨");
        randomMessages.Add("���'�ʺ��'���ջٶԷ��鱨");
        randomMessages.Add("�۲���ҷ�ӳ��ʱ���԰����������");
        randomMessages.Add("�Ҽ���� �ƶѿ�ֱ�ӻ�ø��鱨");
        randomMessages.Add("��ק���Ƶ����ͷ���� ��ֱ���öԷ���ȡ�鱨");
        randomMessages.Add("ʶ�ƹ����� ����Ҫѡ�����");
        randomMessages.Add("���������� ��Ҫѡ�����");
    }

    public void AddMessage(Sprite image, string mainMessage, string subMessage)
    {
        SingleMessage newMessage= Instantiate(singleMessage, MessagePanel);
        if(newMessage !=null)
        {
            if (subMessage.Length < 3) subMessage = RandonMessageProcessor();
            newMessage.SetCardInfo(mainMessage, subMessage, image);
            allMessages.Add(newMessage);
        }
    }

    public void AddMessage(string mainMessage, int spellType)
    {
        string subMessage = SubMessageProcessor(spellType);
        AddMessage(null, mainMessage, subMessage);
    }

    public void DestroyMessage()
    {
        if (allMessages.Count < 1) return;
        Destroy(allMessages[0].gameObject);
        allMessages.RemoveAt(0);
    }

    private string SubMessageProcessor(int spellType)
    {
        switch (spellType)
        {
            case (int)SpellType.Lock:
                return "����������Ҵ˻غϱ�����մ����鱨";
            case (int)SpellType.Away:
                return "��������ɽ����Ҵ˻غϲ��ܽ��մ����鱨";
            case (int)SpellType.Help:
                return "��ȡ[���鱨����+1]�Ŀ���";
            case (int)SpellType.Redirect:
                return "��ת�Ƶ���ұ�����մ��鱨";
            case (int)SpellType.Gamble:
                return "���ƿ��ȡһ������Ϊ�Լ����鱨";
            case (int)SpellType.Intercept:
                return "�ȴ������ѡ������鱨";
            case (int)SpellType.Test:
                return "��̽�����ݵ�ϸ";
            case (int)SpellType.Burn:
                return "һ�������鱨���ջ�";
            case (int)SpellType.Swap:
                return "�������鱨�����";
            case (int)SpellType.Decrypt:
                return "���Դ��鱨��Ϣ ������. �����ȷ,�ɳ���+����";
            case (int)SpellType.GambleAll:
                return "���������ʱ�� ���ƿ��ȡһ���� ��Ϊ�Լ��鱨";
            case (int)SpellType.Cancel:
                return "��ʶ�ƵĹ����� Ч����Ч";
            case (int)SpellType.Trade:
                return "�������п��� ��ȡ�����������";
        }

        return "";
    }

    private string RandonMessageProcessor()
    {
        int randomI = Random.Range(0, randomMessages.Count);
        return "С��ʿ: " + randomMessages[randomI];
    }
}

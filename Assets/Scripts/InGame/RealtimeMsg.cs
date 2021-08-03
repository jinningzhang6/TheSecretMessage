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
    // Start is called before the first frame update
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
            case 0:
                return "����������Ҵ˻غϱ�����մ����鱨";
            case 1:
                return "��������ɽ����Ҵ˻غϲ��ܽ��մ����鱨";
            case 2:
                return "��ȡ[���鱨����+1]�Ŀ���";
            case 3:
                return "��ת�Ƶ���ұ�����մ��鱨";
            case 4:
                return "���ƿ��ȡһ������Ϊ�Լ����鱨";
            case 5:
                return "�ȴ������ѡ������鱨";
            case 6:
                return "��̽�����ݵ�ϸ";
            case 7:
                return "һ�������鱨���ջ�";
            case 8:
                return "�������鱨�����";
            case 9:
                return "���Դ��鱨��Ϣ ������. �����ȷ,�ɳ���+����";
            case 10:
                return "���������ʱ�� ���ƿ��ȡһ���� ��Ϊ�Լ��鱨";
            case 12:
                return "��ʶ�ƵĹ����� Ч����Ч";
            case 13:
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

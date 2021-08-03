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
        randomMessages.Add("右键点击传输中的牌可翻看内容");
        randomMessages.Add("右键双击传输中的牌来公开情报");
        randomMessages.Add("点击'彩虹板'来烧毁对方情报");
        randomMessages.Add("观察玩家反映有时可以帮助鉴定身份");
        randomMessages.Add("右键点击 牌堆可直接获得该情报");
        randomMessages.Add("拖拽卡牌到玩家头像上 可直接让对方获取情报");
        randomMessages.Add("识破功能牌 不需要选择玩家");
        randomMessages.Add("锁定功能牌 需要选择玩家");
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
                return "被锁定的玩家此回合必须接收传输情报";
            case 1:
                return "被调虎离山的玩家此回合不能接收传输情报";
            case 2:
                return "抽取[黑情报数量+1]的卡牌";
            case 3:
                return "被转移的玩家必须接收此情报";
            case 4:
                return "从牌库抽取一张牌作为自己的情报";
            case 5:
                return "等待此玩家选择接收情报";
            case 6:
                return "试探玩家身份底细";
            case 7:
                return "一张已有情报被烧毁";
            case 8:
                return "传输中情报被变更";
            case 9:
                return "宣言此情报信息 并翻开. 如果正确,可抽牌+公开";
            case 10:
                return "所有玩家逆时针 从牌库抽取一张牌 作为自己情报";
            case 12:
                return "被识破的功能牌 效果无效";
            case 13:
                return "丢弃所有卡牌 抽取相等数量卡牌";
        }
        return "";
    }

    private string RandonMessageProcessor()
    {
        int randomI = Random.Range(0, randomMessages.Count);
        return "小贴士: " + randomMessages[randomI];
    }
}

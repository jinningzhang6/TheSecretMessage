
/// <summary>
/// �ᴩ��Ϸ�߼�
/// </summary>
public enum GameEvent {

    /// <summary>
    /// ��ȡ����
    /// </summary>
    DrawCard = 0,

    /// <summary>
    /// ������Ϸ��ʼ
    /// </summary>
    TurnStart = 1,

    /// <summary>
    /// �����鱨�����
    /// </summary>
    SendCard = 2,

    // ĳ��Ҵ��������
    SpellCard = 3,

    // ����鱨�ѽ���
    ToEndTurn = 4,

    // ����ʣ����
    CardsLeft = 5,//Deprecated

    // �鱨����
    SuccessReceive = 6,

    /// <summary>
    /// ����Ҵ������� �� �ӵ����ƶ�
    /// </summary>
    DropCard = 7,

    // ���� �������С����鱨
    OpenCard = 8,

    /// <summary>
    /// ���˲��ɹ������鱨
    /// </summary>
    DirectReceive = 9
}

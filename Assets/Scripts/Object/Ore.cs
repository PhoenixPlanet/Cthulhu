using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : MonoBehaviour
{
    //ü���� 0�̵Ǹ� ������ ����
    //������ ����
    #region PublicVariables
    #endregion

    #region PrivateVariables
    private int Hp;
    #endregion

    #region PublicMethod
    public void Hit(int PlayerAtk) //�Ű�����
    {
        //�÷��̾��� ���ݷ¿� ���� ü���� ����
        Hp -= PlayerAtk;
        if (Hp <= 0)
        {
            DestroyOre();
        }
    }
    #endregion

    #region PrivateMethod
    private void DestroyOre()
    {
        Destroy(gameObject);
    }
    #endregion
}

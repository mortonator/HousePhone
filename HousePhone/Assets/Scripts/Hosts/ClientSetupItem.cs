using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientSetupItem : MonoBehaviour
{
    [SerializeField] Image[] avatar;
    [SerializeField] TMPro.TextMeshProUGUI nameText;
    [Header("Ready")]
    [SerializeField] Image readyImage;
    [SerializeField] Sprite ready_spr;
    [SerializeField] Sprite readyNot_spr;

    int actor;
    public void Setup(int actNum) => actor = actNum;

    public bool UpdateItem()
    {
        if (GameManager.Instance.clientManagers.ContainsKey(actor) && GameManager.Instance.clientManagers[actor].setup.avatarIndex == null)
        {
            if (GameManager.Instance.clientManagers.ContainsKey(actor))
                Debug.Log(actor + " doesn't exist in client managers");
            else
                Debug.Log(actor + " client's avatarIndex array is null");

            return false;
        }

        nameText.text = GameManager.Instance.clientManagers[actor].setup.inputName;

        SetSprite(0);
        SetSprite(1);
        SetSprite(2);
        SetSprite(3);

        if (GameManager.Instance.clientManagers[actor].setup.isReady)
            readyImage.sprite = ready_spr;
        else
            readyImage.sprite = readyNot_spr;

        return GameManager.Instance.clientManagers[actor].setup.isReady;
    }
    void SetSprite(int i) => avatar[i].sprite = GameManager.GetAvatarPart(i, GameManager.Instance.clientManagers[actor].setup.avatarIndex[i]);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class iconController : MonoBehaviour
{
    // プレイヤーオブジェクトの名前
    [SerializeField] string playerObjName = null;
    GameObject player = null;   // プレイヤーオブジェクト

    public Vector3 screenPos;
    public Vector3 playerPos;

    Image thisImage;

    float time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
        player = GameObject.Find(playerObjName);
        thisImage = this.gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        playerPos = player.transform.position;
        playerPos.y += 2.5f;
        screenPos = Camera.main.WorldToScreenPoint(playerPos);

        thisImage.rectTransform.position = screenPos;
    }
}

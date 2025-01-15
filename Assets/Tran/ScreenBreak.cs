using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class ScreenBreak : MonoBehaviour
{
    public Material mat;
    public Camera cameraToUse; // 保存するカメラ
    private string savePath = "Assets/Resources/SavedImage.png"; // 保存先のファイルパス
    public Texture2D tex;

    private bool onFlag;
    async void Start()
    {
        GameManager.Instance.sb = this.gameObject;
        DontDestroyOnLoad(this);
    }
    
    public async Task FastBreak()
    {
        ScreenShot cam =Camera.main.GetComponent<ScreenShot>();
        
        cam.StartSnap();
        mat.SetTexture("_MainTex",cam.tex);
		Vector3 camPos = cam.transform.position;
        // モデルのピポットを真ん中にしていないために、強制的にずらす
		transform.position = new Vector3(camPos.x + 10.12f, camPos.y - 5.65f, transform.position.z);
		//Destroy(tex);
		mat.SetFloat("distortedTime",0.01f);
        await Task.Delay(1000);
    }
    

    public async Task StartBreak(float time)
    {
        float current = 0.0f;
        while (current <= time)
        {
            mat.SetFloat("distortedTime",Mathf.Lerp(0f,5f,current / time));
            current += Time.deltaTime;
            await Task.Yield();
        }
        Destroy(this.gameObject);
        
        
    }
}

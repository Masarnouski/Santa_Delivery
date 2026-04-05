using UnityEngine;
using InputSystemKeyboard = UnityEngine.InputSystem.Keyboard;
using InputSystemTouchscreen = UnityEngine.InputSystem.Touchscreen;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class SantaController : MonoBehaviour {
    [Header("Movement")] public float moveSpeed=3f; public float minY=1f; public float maxY=6f;
    [Header("Gift")] public GameObject giftPrefab; public float giftGravity=12f; public float giftDriftX=-1.5f;
    struct GD{public GameObject obj;public float vx;public float vy;public SpriteRenderer sr;}
    List<GD> gs=new List<GD>(); float cb,cl,tv,gc; bool tg; int aid=-1; Vector2 to;
    void Start(){Camera c=Camera.main;cb=c.transform.position.y-c.orthographicSize-3f;cl=c.transform.position.x-c.orthographicSize*c.aspect-3f;}
    void Update(){
        gc-=Time.deltaTime; tv=0f; tg=false;
        var ts=InputSystemTouchscreen.current;
        if(ts!=null){float hw=Screen.width*.5f;var arr=ts.touches;bool f=false;
            for(int i=0;i<arr.Count;i++){var t=arr[i];var ph=t.phase.ReadValue();var pos=t.position.ReadValue();bool L=pos.x<=hw;bool R=pos.x>hw;
                if(ph==UnityEngine.InputSystem.TouchPhase.Began){if(R&&!f){tg=true;f=true;}if(L&&aid==-1){aid=t.touchId.ReadValue();to=pos;}}
                if(L&&t.touchId.ReadValue()==aid){if(ph==UnityEngine.InputSystem.TouchPhase.Moved||ph==UnityEngine.InputSystem.TouchPhase.Stationary){float d=pos.y-to.y;if(Mathf.Abs(d)>=20f)tv=d>0f?1f:-1f;}if(ph==UnityEngine.InputSystem.TouchPhase.Ended||ph==UnityEngine.InputSystem.TouchPhase.Canceled)aid=-1;}}
            bool any=false;foreach(var t in arr)if(t.isInProgress){any=true;break;}if(!any)aid=-1;}
        float kv=0f;bool sp=false;var kb=InputSystemKeyboard.current;
        if(kb!=null){if(kb.wKey.isPressed||kb.upArrowKey.isPressed)kv=1f;else if(kb.sKey.isPressed||kb.downArrowKey.isPressed)kv=-1f;sp=kb.spaceKey.wasPressedThisFrame;}
        float v=kv!=0f?kv:tv; Vector3 p=transform.position; p.y+=v*moveSpeed*Time.deltaTime; p.y=Mathf.Clamp(p.y,minY,maxY); transform.position=p;
        if((sp||tg)&&giftPrefab!=null&&gc<=0f){Fire(giftDriftX,0f);gc=0.3f;}
        for(int i=gs.Count-1;i>=0;i--){GD g=gs[i];if(g.obj==null){gs.RemoveAt(i);continue;}g.vy-=giftGravity*Time.deltaTime;Vector3 gp=g.obj.transform.position;gp.x+=g.vx*Time.deltaTime;gp.y+=g.vy*Time.deltaTime;g.obj.transform.position=gp;gs[i]=g;
            if(GameManager.Instance!=null){HouseMover tgt=GameManager.Instance.TargetHouse;if(tgt!=null&&tgt.IsTarget&&tgt.GetComponent<SpriteRenderer>().bounds.Contains(new Vector3(gp.x,gp.y,0f))){tgt.ClearTarget();GameManager.Instance.OnGiftDelivered();Destroy(g.obj);gs.RemoveAt(i);continue;}}
            if(gp.y<cb||gp.x<cl){Destroy(g.obj);gs.RemoveAt(i);}}}
    void Fire(float vx,float vy){if(giftPrefab==null)return;GameObject ng=Instantiate(giftPrefab,transform.position,Quaternion.identity);SpriteRenderer sr=ng.GetComponent<SpriteRenderer>();if(GameManager.Instance!=null){var gl=ng.AddComponent<GlowEffect>();gl.SetGlow(GameManager.Instance.CurrentTargetColor);}gs.Add(new GD{obj=ng,vx=vx,vy=vy,sr=sr});}
    public void OnHitByEnemy(){if(GameManager.Instance!=null)GameManager.Instance.OnEnemyHit();float[]a={-120f,-90f,-60f};for(int k=0;k<3;k++){float r=a[k]*Mathf.Deg2Rad;Fire(Mathf.Cos(r)*3.5f+giftDriftX,Mathf.Sin(r)*3.5f);}}
}
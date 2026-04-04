using UnityEngine;
public class SnowEffect : MonoBehaviour {
    public int snowCount=150; public float areaWidth=22f; public float spawnY=6f;
    public float minSpeed=1.2f; public float maxSpeed=2.8f; public float minSize=0.04f; public float maxSize=0.12f;
    struct F{public Transform tr;public float sp;public float df;public float dof;public float sx;}
    F[] fs; float ky; Sprite spr;
    void Start(){ky=Camera.main.transform.position.y-Camera.main.orthographicSize-1f;spr=Dot();fs=new F[snowCount];for(int i=0;i<snowCount;i++)fs[i]=Mk(true);}
    void Update(){for(int i=0;i<fs.Length;i++){if(fs[i].tr==null){fs[i]=Mk(false);continue;}Vector3 p=fs[i].tr.position;p.y-=fs[i].sp*Time.deltaTime;p.x=fs[i].sx+Mathf.Sin(Time.time*fs[i].df+fs[i].dof)*0.3f;fs[i].tr.position=p;if(p.y<ky){Destroy(fs[i].tr.gameObject);fs[i]=Mk(false);}}}
    F Mk(bool sc){GameObject g=new GameObject("_s");SpriteRenderer r=g.AddComponent<SpriteRenderer>();r.sprite=spr;r.sortingOrder=50;r.color=new Color(1f,1f,1f,Random.Range(0.5f,0.95f));float sz=Random.Range(minSize,maxSize);g.transform.localScale=Vector3.one*sz;float x=Random.Range(-areaWidth*0.5f,areaWidth*0.5f);float y=sc?Random.Range(ky,spawnY):spawnY+0.5f;g.transform.position=new Vector3(x,y,-1f);return new F{tr=g.transform,sp=Random.Range(minSpeed,maxSpeed),df=Random.Range(0.5f,2f),dof=Random.Range(0f,6.28f),sx=x};}
    Sprite Dot(){int n=16;float c=n*0.5f;Texture2D t=new Texture2D(n,n,TextureFormat.RGBA32,false);t.filterMode=FilterMode.Bilinear;for(int y=0;y<n;y++)for(int x=0;x<n;x++){float d=Mathf.Sqrt((x-c)*(x-c)+(y-c)*(y-c));t.SetPixel(x,y,new Color(1f,1f,1f,Mathf.Clamp01(1f-(d-c+1f))));}t.Apply();return Sprite.Create(t,new Rect(0,0,n,n),Vector2.one*0.5f,n);}
    void OnDestroy(){if(fs==null)return;foreach(var f in fs)if(f.tr!=null)Destroy(f.tr.gameObject);}
}

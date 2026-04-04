using UnityEngine;
public class GlowEffect : MonoBehaviour {
    static readonly float[] GS={1.10f,1.22f,1.38f,1.58f};
    static readonly float[] GA={0.55f,0.30f,0.15f,0.06f};
    GameObject[] gl; SpriteRenderer src;
    public void SetGlow(Color c){src=GetComponent<SpriteRenderer>();if(src==null)return;RemoveLayers();gl=new GameObject[GS.Length];for(int i=0;i<GS.Length;i++){var g=new GameObject("_g"+i);g.transform.SetParent(transform,false);g.transform.localPosition=Vector3.zero;g.transform.localScale=Vector3.one*GS[i];var sr=g.AddComponent<SpriteRenderer>();sr.sprite=src.sprite;sr.sortingLayerID=src.sortingLayerID;sr.sortingOrder=src.sortingOrder-1-i;sr.color=new Color(1f,0.88f,0.05f,GA[i]);gl[i]=g;}}
    public void ClearGlow(){RemoveLayers();}
    void RemoveLayers(){if(gl==null)return;foreach(var g in gl)if(g!=null)Destroy(g);gl=null;}
    void OnDestroy(){RemoveLayers();}
}
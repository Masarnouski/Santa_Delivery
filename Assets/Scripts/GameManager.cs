using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour {
    public static GameManager Instance{get;private set;}
    public LivesDisplay livesDisplay;
    public TextMeshProUGUI scoreText;
    public int maxLives=3;
    public string[] names=new string[]{"Alice","Bob","Vera","Gleb","Dasha","Egor","Zhanna","Ivan","Katya","Lesha","Masha","Nikita","Olya","Petya","Sveta"};
    public Color[] palette=new Color[]{new Color(1f,.2f,.2f),new Color(.2f,.8f,.2f),new Color(.2f,.6f,1f),new Color(1f,.8f,.1f),new Color(.9f,.3f,1f),new Color(.1f,.9f,.9f),new Color(1f,.5f,.1f)};
    public string CurrentTargetName{get;private set;}
    public Color CurrentTargetColor{get;private set;}
    public HouseMover TargetHouse{get;private set;}
    int ci=0; int hp; bool dead=false; int delivered=0;
    void Awake(){Instance=this;}
    void Start(){hp=maxLives;if(livesDisplay!=null)livesDisplay.UpdateHearts(hp);if(scoreText!=null)scoreText.text="Delivered: 0";NextTarget();}
    public void OnEnemyHit(){if(dead)return;hp=Mathf.Max(0,hp-1);if(livesDisplay!=null)livesDisplay.UpdateHearts(hp);if(hp==0){dead=true;if(livesDisplay!=null)livesDisplay.ShowGameOver();}}
    public void NextTarget(){CurrentTargetName=names[Random.Range(0,names.Length)];CurrentTargetColor=palette[ci%palette.Length];ci++;TargetHouse=null;}
    public void RegisterHouse(HouseMover h){if(TargetHouse==null||!TargetHouse.IsTarget){TargetHouse=h;h.SetAsTarget(CurrentTargetColor);}}
    public void UnregisterHouse(HouseMover h){if(TargetHouse==h)TargetHouse=null;}
    public void OnGiftDelivered(){delivered++;if(scoreText!=null)scoreText.text="Delivered: "+delivered;NextTarget();}
}

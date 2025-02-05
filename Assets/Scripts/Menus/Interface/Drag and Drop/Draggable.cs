using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Draggable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // Script se trouvant dans les objets (item) qui peuvent être déplacés de slot en slot
    // à l'aide de la fonctionnalité drag and drop


    private Image image; // on en a de besoin pour jouer avec sa propriété raycast target
    [HideInInspector] public Transform parentInitial;
    [HideInInspector] public Transform parentFinal;
    public bool peutDrag; // permet de savoir si cet item peut être déplacer ou non

    // Les deux canevas parents
    Transform transformCanvasInventaire;
    Transform transformCanvasCraft;

    void Awake()
    {
        image = GetComponent<Image>();

        // Si l'objet est dans le canevas de l'inventaire
        GameObject canvasInventaire = GameObject.FindGameObjectWithTag("Canvas_Inventaire");
        if(canvasInventaire != null ) transformCanvasInventaire = canvasInventaire.GetComponent<Transform>();

        // Si l'objet est dans le canevas de construction
        GameObject canvasCraft = GameObject.FindGameObjectWithTag("CanvasCraft");
        if (canvasCraft != null) transformCanvasCraft = canvasCraft.GetComponent<Transform>();
    }

    // Méthode qui sera appelé lorsque le joueur clique sur l'objet qui peut être déplacé
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentInitial = transform.parent; // assigne son parent initial comme son parent actuel
        parentFinal = transform.parent; // assigne son parent final comme son parent actuel au cas ou il ne soit pas redirigé vers un autre slot

        if (peutDrag)
        {
            ComportementInterface.instance.draggableActuel = this;

            // En fonction de dans quel canevas il est contenu
            if (transformCanvasInventaire != null && transformCanvasInventaire.gameObject.activeInHierarchy)
            {
                transform.SetParent(transformCanvasInventaire);

            }
            else
            {
                transform.SetParent(transformCanvasCraft);
            }


            transform.SetAsLastSibling(); // place l'objet à la fin de la hierachie pour qu'il soit visible 
            image.raycastTarget = false; // pour éviter des interférences futurs
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (peutDrag)
        {
            transform.position = Input.mousePosition; // l'objet suit la souris
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        transform.SetParent(parentFinal); // assigne le slot sur lequel il a été pointé
        transform.SetSiblingIndex(1); // le place au bon endroit dans la hierachie
        gameObject.GetComponent<Item>().AfficherInfos(false); // pour éviter que les infos apparaissent à la fin (choix de design)
        transform.position = parentFinal.position; // le place au même endroit que son parent
        image.raycastTarget = true; // réactive son raycastTarget pour qu'on puisse interragir avec lui à nouveau
      
    }

}

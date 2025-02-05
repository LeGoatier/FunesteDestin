using System.Collections;
using TMPro;
using UnityEngine;

public class MortCanevas : MonoBehaviour
{
    [SerializeField] float duréeActivationCanevas = 5;
    [SerializeField] string[] messagesEncouragement;
    [SerializeField] TextMeshProUGUI messageAffiché;
    private void OnEnable()
    {
        StartCoroutine(AttendreEtDésactiver());
        ChoisirMessageEncouragement();
    }
    private void ChoisirMessageEncouragement()
    {
        messageAffiché.text = messagesEncouragement[UnityEngine.Random.Range(0, messagesEncouragement.Length)];
    }
    private IEnumerator AttendreEtDésactiver()
    {
        yield return new WaitForSeconds(duréeActivationCanevas);
        FinManager.instance.LancerRésuméPartie();
        gameObject.SetActive(false);
    }
}

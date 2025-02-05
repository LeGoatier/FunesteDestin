using System.Collections;
using TMPro;
using UnityEngine;

public class MortCanevas : MonoBehaviour
{
    [SerializeField] float dur�eActivationCanevas = 5;
    [SerializeField] string[] messagesEncouragement;
    [SerializeField] TextMeshProUGUI messageAffich�;
    private void OnEnable()
    {
        StartCoroutine(AttendreEtD�sactiver());
        ChoisirMessageEncouragement();
    }
    private void ChoisirMessageEncouragement()
    {
        messageAffich�.text = messagesEncouragement[UnityEngine.Random.Range(0, messagesEncouragement.Length)];
    }
    private IEnumerator AttendreEtD�sactiver()
    {
        yield return new WaitForSeconds(dur�eActivationCanevas);
        FinManager.instance.LancerR�sum�Partie();
        gameObject.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class EnergySystemUIController : MonoBehaviour
{
    public Text LivesAmountLabel => _livesAmountLabel;
    public Text RestoreTimeEstimate => _restoreTimeEstimate;
    
    [SerializeField]
    private Text _livesAmountLabel;
    [SerializeField]
    private Text _restoreTimeEstimate;
}

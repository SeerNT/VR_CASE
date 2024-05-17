using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Path : MonoBehaviour
{
    [Header("Attachments")]
    [SerializeField] private GameObject delivery;
    private GameObject objForDelivery;
    [SerializeField] private GameObject deskWorkpieces;
    [SerializeField] private GameObject deskDetails;
    [SerializeField] private GameObject deskMain;
    [SerializeField] private GameObject detailPrefab;
    [Header("Settings")]
    [SerializeField] private GameObject[] pathPoints;
    [SerializeField] private float[] timings;
    [SerializeField] private float speed;

    private Vector3 actualPos;
    private int x;
    [Header("UI")]
    [SerializeField] private Button startBut;
    [SerializeField] private TextMeshProUGUI stageText;

    private bool readyForNextPhase = false;

    Dictionary<int, string> stagesDict = new Dictionary<int, string>()
    {
        {0, "Ожидание" },
        {1, "Доставка заготовок" },
        {2, "Получение заготовок" },
        {3, "Ожидание деталей" },
        {4, "Получение деталей" },
        {5, "Доставка деталей" },
        {6, "Возвращение" }
    };
    // -242 293 921
    private void Start()
    {
        x = 0;

        stageText.text = stagesDict[x];

        startBut.onClick.AddListener(delegate { StartCoroutine(NextPhase(timings[x])); });
    }

    public IEnumerator NextPhase(float time)
    {
        yield return new WaitForSeconds(time);

        x++;
        readyForNextPhase = true;

        stageText.text = stagesDict[x];

        if (x == pathPoints.Length - 1)
        {
            objForDelivery.transform.SetParent(deskDetails.transform);
            objForDelivery.transform.localPosition = new Vector3(1.145f, 0.481f, 0.229f);
            readyForNextPhase = false;

            objForDelivery = null;
            x = 0;
            stageText.text = stagesDict[x];
        }

        if(x == 1)
        {
            objForDelivery = deskWorkpieces.transform.GetChild(0).gameObject;
            objForDelivery.transform.SetParent(delivery.transform);
            objForDelivery.transform.localPosition = new Vector3(-242, 293, 921);
        } 
        if(x == 3)
        {
            objForDelivery.transform.SetParent(deskMain.transform);
            objForDelivery.transform.localPosition = new Vector3(0.626f, 0.481f, -0.06f);
        }
        if(x == 4)
        {
            objForDelivery = detailPrefab;
        }
    }

    private void Update()
    {
        actualPos = delivery.transform.position;
        delivery.transform.position = Vector3.MoveTowards(actualPos, 
            new Vector3(pathPoints[x].transform.position.x, 0.09f, pathPoints[x].transform.position.z),
            speed * Time.deltaTime);

        if(actualPos == new Vector3(pathPoints[x].transform.position.x, 0.09f, pathPoints[x].transform.position.z)
            && x != pathPoints.Length - 1 && readyForNextPhase)
        {
            readyForNextPhase = false;
            StartCoroutine(NextPhase(timings[x]));
        }
    }
}

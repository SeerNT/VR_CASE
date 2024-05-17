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
    [SerializeField] private Animator manipulatorAnimator;
    [SerializeField] private GameObject manipulator;
    [SerializeField] private GameObject machine;
    [SerializeField] private Animator mold;
    [Header("Settings")]
    [SerializeField] private GameObject[] pathPoints;
    [SerializeField] private float[] timings;
    [SerializeField] private float speed;
    [SerializeField] private float[] detailsPosX;

    private Vector3 actualPos;
    private int stage;

    private int detailsAmount = 0;

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
        {5, "Доставка деталей" }
    };

    private void Start()
    {
        stage = 0;

        stageText.text = stagesDict[stage];

        startBut.onClick.AddListener(StartCycle);
    }

    public void StartCycle()
    {
        if (stage == 0 && detailsAmount < 10)
            StartCoroutine(NextPhase(timings[stage]));
    }

    IEnumerator NextPhase(float time)
    {
        yield return new WaitForSeconds(time);

        stage++;
        readyForNextPhase = true;



        if (stage == pathPoints.Length - 1)
        {
            objForDelivery.transform.SetParent(deskDetails.transform);

            detailsAmount++;

            if ((detailsAmount) % 2 == 0)
            {
                objForDelivery.transform.localPosition = new Vector3(detailsPosX[detailsAmount], 0.481f, -0.192f);
            }
            else
            {
                objForDelivery.transform.localPosition = new Vector3(detailsPosX[detailsAmount], 0.481f, 0.229f);
            }


            readyForNextPhase = false;

            objForDelivery = null;
            stage = 0;
            stageText.text = stagesDict[stage];


        }
        else
        {
            stageText.text = stagesDict[stage];
        }

        if(stage == 1)
        {
            objForDelivery = deskWorkpieces.transform.GetChild(0).gameObject;
            objForDelivery.transform.SetParent(delivery.transform);
            objForDelivery.transform.localPosition = new Vector3(-242, 293, 921);
        } 
        if(stage == 2)
        {
            // Wait for robot arriving
            yield return new WaitForSeconds(1.5f);
            manipulatorAnimator.Play("RotateToPickUp");
            yield return new WaitForSeconds(0.6f);
            objForDelivery.transform.SetParent(manipulator.transform);
            objForDelivery.transform.localPosition = new Vector3(-1045.7f, 990.8f, 252.5f);
            objForDelivery.transform.localEulerAngles = new Vector3(40.397f, 96.159f, 4.339f);
            yield return new WaitForSeconds(1.1f);
            objForDelivery.transform.SetParent(deskMain.transform);
            objForDelivery.transform.localPosition = new Vector3(0.626f, 0.481f, -0.06f);
            objForDelivery.transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        if(stage == 3)
        {
            objForDelivery.transform.SetParent(manipulator.transform);
            objForDelivery.transform.localPosition = new Vector3(-1045.7f, 990.8f, 252.5f);
            objForDelivery.transform.localEulerAngles = new Vector3(40.397f, 96.159f, 4.339f);
            yield return new WaitForSeconds(1.2f);
            manipulatorAnimator.Play("RotateToSend");
            yield return new WaitForSeconds(0.6f);
            objForDelivery.transform.SetParent(machine.transform);
            objForDelivery.transform.localPosition = new Vector3(-1979f, 2549f, 2398f);
            objForDelivery.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            objForDelivery.transform.localScale = new Vector3(200f, 200f, 200f);
            yield return new WaitForSeconds(0.8f);
            mold.Play("Press");
            yield return new WaitForSeconds(0.9f);
            Destroy(objForDelivery.transform.GetChild(0).gameObject);
            Instantiate(detailPrefab, objForDelivery.transform);
            yield return new WaitForSeconds(0.6f);
            manipulatorAnimator.Play("RotateToDeskFromMachine");
            yield return new WaitForSeconds(0.6f);
            objForDelivery.transform.SetParent(manipulator.transform);
            objForDelivery.transform.localPosition = new Vector3(-1045.7f, 990.8f, 252.5f);
            objForDelivery.transform.localEulerAngles = new Vector3(40.397f, 96.159f, 4.339f);
            yield return new WaitForSeconds(1.2f);
            objForDelivery.transform.SetParent(deskMain.transform);
            objForDelivery.transform.localPosition = new Vector3(0.626f, 0.481f, -0.06f);
            objForDelivery.transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        if(stage == 4)
        {
            // Wait for robot arriving
            yield return new WaitForSeconds(1.5f);
            objForDelivery.transform.SetParent(manipulator.transform);
            objForDelivery.transform.localPosition = new Vector3(-1045.7f, 990.8f, 252.5f);
            objForDelivery.transform.localEulerAngles = new Vector3(40.397f, 96.159f, 4.339f);
            manipulatorAnimator.Play("RotateToPickUp");
            yield return new WaitForSeconds(1.1f);
            objForDelivery.transform.SetParent(delivery.transform);
            objForDelivery.transform.localPosition = new Vector3(-242, 293, 921);
        }
        if(stage == 5)
        {
            objForDelivery.transform.SetParent(delivery.transform);
            objForDelivery.transform.localPosition = new Vector3(-242, 293, 921);
        }
    }

    private void Update()
    {
        actualPos = delivery.transform.position;
        delivery.transform.position = Vector3.MoveTowards(actualPos, 
            new Vector3(pathPoints[stage].transform.position.x, 0.09f, pathPoints[stage].transform.position.z),
            speed * Time.deltaTime);

        if(actualPos == new Vector3(pathPoints[stage].transform.position.x, 0.09f, pathPoints[stage].transform.position.z)
            && stage != pathPoints.Length - 1 && readyForNextPhase)
        {
            readyForNextPhase = false;
            StartCoroutine(NextPhase(timings[stage]));
        }
    }
}

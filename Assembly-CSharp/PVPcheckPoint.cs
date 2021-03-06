using System.Collections;
using UnityEngine;

public class PVPcheckPoint : Photon.MonoBehaviour
{
    private FengGameManagerMKII fengGame;
    public int id;
    public GameObject humanCyc;
    public GameObject titanCyc;
    public CheckPointState state;
    private bool titanOn;
    private bool playerOn;
    public float humanPt;
    public float titanPt;
    public float humanPtMax = 40f;
    public float titanPtMax = 40f;
    public int normalTitanRate = 70;
    private bool annie;
    private float spawnTitanTimer;
    public float titanInterval = 30f;
    public float size = 1f;
    private float hitTestR = 15f;
    private GameObject supply;
    private float syncTimer;
    private float syncInterval = 0.6f;
    private float getPtsTimer;
    private float getPtsInterval = 20f;
    public GameObject[] chkPtNextArr;
    public GameObject[] chkPtPreviousArr;
    public static ArrayList chkPts;
    public bool hasAnnie;
    public bool isBase;

    public GameObject chkPtNext
    {
        get
        {
            if (chkPtNextArr.Length <= 0)
            {
                return null;
            }
            return chkPtNextArr[Random.Range(0, chkPtNextArr.Length)];
        }
    }

    public GameObject chkPtPrevious
    {
        get
        {
            if (chkPtPreviousArr.Length <= 0)
            {
                return null;
            }
            return chkPtPreviousArr[Random.Range(0, chkPtPreviousArr.Length)];
        }
    }

    private void Start()
    {
        fengGame = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>();
        if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
        {
            Object.Destroy(base.gameObject);
            return;
        }
        if (FengGameManagerMKII.Level.Mode != GameMode.PVP_CAPTURE)
        {
            Object.Destroy(base.gameObject);
            return;
        }
        chkPts.Add(this);
        IComparer comparer = new IComparerPVPchkPtID();
        chkPts.Sort(comparer);
        if (humanPt == humanPtMax)
        {
            state = CheckPointState.Human;
            if (base.photonView.isMine && FengGameManagerMKII.Level.Map != "The City I")
            {
                Vector3 position = base.transform.position;
                Vector3 up = Vector3.up;
                Vector3 position2 = base.transform.position;
                supply = PhotonNetwork.Instantiate("aot_supply", position - up * (position2.y - GetHeight(base.transform.position)), base.transform.rotation, 0);
            }
        }
        else if (base.photonView.isMine && !hasAnnie)
        {
            if (Random.Range(0, 100) < 50)
            {
                int num = Random.Range(1, 2);
                for (int i = 0; i < num; i++)
                {
                    NewTitan();
                }
            }
            if (isBase)
            {
                NewTitan();
            }
        }
        if (titanPt == titanPtMax)
        {
            state = CheckPointState.Titan;
        }
        hitTestR = 15f * size;
        base.transform.localScale = new Vector3(size, size, size);
    }

    private void NewTitan()
    {
        int rate = normalTitanRate;
        Vector3 position = base.transform.position;
        Vector3 up = Vector3.up;
        Vector3 position2 = base.transform.position;
        GameObject gameObject = fengGame.SpawnTitan(rate, position - up * (position2.y - GetHeight(base.transform.position)), base.transform.rotation);
        if (FengGameManagerMKII.Level.Map == "The City I")
        {
            gameObject.GetComponent<TITAN>().chaseDistance = 120f;
        }
        else
        {
            gameObject.GetComponent<TITAN>().chaseDistance = 200f;
        }
        gameObject.GetComponent<TITAN>().PVPfromCheckPt = this;
    }

    private float GetHeight(Vector3 pt)
    {
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
        LayerMask layerMask2 = layerMask;
        if (Physics.Raycast(pt, -Vector3.up, out RaycastHit hitInfo, 1000f, layerMask2.value))
        {
            Vector3 point = hitInfo.point;
            return point.y;
        }
        return 0f;
    }

    private void Update()
    {
        float num = humanPt / humanPtMax;
        float num2 = titanPt / titanPtMax;
        if (!base.photonView.isMine)
        {
            humanCyc.transform.localScale = new Vector3(num, num, 1f);
            titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
            syncTimer += Time.deltaTime;
            if (syncTimer > syncInterval)
            {
                syncTimer = 0f;
                checkIfBeingCapture();
            }
            return;
        }
        switch (state)
        {
            case CheckPointState.None:
                if (playerOn && !titanOn)
                {
                    humanGetsPoint();
                    titanLosePoint();
                }
                else if (titanOn && !playerOn)
                {
                    titanGetsPoint();
                    humanLosePoint();
                }
                else
                {
                    humanLosePoint();
                    titanLosePoint();
                }
                break;
            case CheckPointState.Human:
                if (titanOn && !playerOn)
                {
                    titanGetsPoint();
                }
                else
                {
                    titanLosePoint();
                }
                getPtsTimer += Time.deltaTime;
                if (getPtsTimer > getPtsInterval)
                {
                    getPtsTimer = 0f;
                    if (!isBase)
                    {
                        fengGame.PVPhumanScore++;
                    }
                    fengGame.CheckPvPPoints();
                }
                break;
            case CheckPointState.Titan:
                if (playerOn && !titanOn)
                {
                    humanGetsPoint();
                }
                else
                {
                    humanLosePoint();
                }
                getPtsTimer += Time.deltaTime;
                if (getPtsTimer > getPtsInterval)
                {
                    getPtsTimer = 0f;
                    if (!isBase)
                    {
                        fengGame.PVPtitanScore++;
                    }
                    fengGame.CheckPvPPoints();
                }
                spawnTitanTimer += Time.deltaTime;
                if (spawnTitanTimer > titanInterval)
                {
                    spawnTitanTimer = 0f;
                    if (FengGameManagerMKII.Level.Map == "The City I")
                    {
                        if (GameObject.FindGameObjectsWithTag("titan").Length < 12)
                        {
                            NewTitan();
                        }
                    }
                    else if (GameObject.FindGameObjectsWithTag("titan").Length < 20)
                    {
                        NewTitan();
                    }
                }
                break;
        }
        syncTimer += Time.deltaTime;
        if (syncTimer > syncInterval)
        {
            syncTimer = 0f;
            checkIfBeingCapture();
            SyncPoints();
        }
        num = humanPt / humanPtMax;
        num2 = titanPt / titanPtMax;
        humanCyc.transform.localScale = new Vector3(num, num, 1f);
        titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
    }

    private void checkIfBeingCapture()
    {
        playerOn = false;
        titanOn = false;
        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < heroes.Length; i++)
        {
            if (!(Vector3.Distance(heroes[i].transform.position, base.transform.position) < hitTestR))
            {
                continue;
            }
            playerOn = true;
            if (state == CheckPointState.Human && heroes[i].GetPhotonView().isMine)
            {
                if (fengGame.checkpoint != base.gameObject)
                {
                    fengGame.checkpoint = base.gameObject;
                    InRoomChat.Instance.AddLine(("Respawn point changed to Point #" + id).WithColor("A8FF24"));
                }
                break;
            }
        }

        GameObject[] titans = GameObject.FindGameObjectsWithTag("titan");
        for (int i = 0; i < titans.Length; i++)
        {
            TITAN titan = titans[i].GetComponent<TITAN>();
            if (Vector3.Distance(titans[i].transform.position, base.transform.position) < hitTestR + 5f && (!titan || !titan.hasDie))
            {
                titanOn = true;
                if (state == CheckPointState.Titan && titan.photonView.isMine && (bool)titan && titan.nonAI)
                {
                    if (fengGame.checkpoint != base.gameObject)
                    {
                        fengGame.checkpoint = base.gameObject;
                        InRoomChat.Instance.AddLine(("Respawn point changed to Point #" + id).WithColor("A8FF24"));
                    }
                    break;
                }
            }
        }
    }

    private void humanGetsPoint()
    {
        if (humanPt >= humanPtMax)
        {
            humanPt = humanPtMax;
            titanPt = 0f;
            SyncPoints();
            state = CheckPointState.Human;
            base.photonView.RPC("changeState", PhotonTargets.AllBuffered, (int)CheckPointState.Human);
            if (FengGameManagerMKII.Level.Map != "The City I")
            {
                Vector3 position = base.transform.position;
                Vector3 up = Vector3.up;
                Vector3 position2 = base.transform.position;
                supply = PhotonNetwork.Instantiate("aot_supply", position - up * (position2.y - GetHeight(base.transform.position)), base.transform.rotation, 0);
            }
            fengGame.PVPhumanScore += 2;
            fengGame.CheckPvPPoints();
            if (HasTeamWon(CheckPointState.Human))
            {
                fengGame.WinGame();
            }
        }
        else
        {
            humanPt += Time.deltaTime;
        }
    }

    private void titanGetsPoint()
    {
        if (titanPt >= titanPtMax)
        {
            titanPt = titanPtMax;
            humanPt = 0f;
            SyncPoints();
            if (state == CheckPointState.Human && supply != null)
            {
                PhotonNetwork.Destroy(supply);
            }
            state = CheckPointState.Titan;
            base.photonView.RPC("changeState", PhotonTargets.AllBuffered, (int)CheckPointState.Titan);
            fengGame.PVPtitanScore += 2;
            fengGame.CheckPvPPoints();
            if (HasTeamWon(CheckPointState.Titan))
            {
                fengGame.LoseGame();
            }
            if (hasAnnie)
            {
                if (!annie)
                {
                    annie = true;
                    Vector3 position = base.transform.position;
                    Vector3 up = Vector3.up;
                    Vector3 position2 = base.transform.position;
                    PhotonNetwork.Instantiate("FEMALE_TITAN", position - up * (position2.y - GetHeight(base.transform.position)), base.transform.rotation, 0);
                }
                else
                {
                    NewTitan();
                }
            }
            else
            {
                NewTitan();
            }
        }
        else
        {
            titanPt += Time.deltaTime;
        }
    }

    private void titanLosePoint()
    {
        if (!(titanPt > 0f))
        {
            return;
        }
        titanPt -= Time.deltaTime * 3f;
        if (titanPt <= 0f)
        {
            titanPt = 0f;
            SyncPoints();
            if (state != CheckPointState.Human)
            {
                base.photonView.RPC("changeState", PhotonTargets.AllBuffered, (int)CheckPointState.None);
            }
        }
    }

    private void humanLosePoint()
    {
        if (!(humanPt > 0f))
        {
            return;
        }
        humanPt -= Time.deltaTime * 3f;
        if (humanPt <= 0f)
        {
            humanPt = 0f;
            SyncPoints();
            if (state != CheckPointState.Titan)
            {
                base.photonView.RPC("changeState", PhotonTargets.AllBuffered, (int)CheckPointState.None);
            }
        }
    }

    private void SyncPoints()
    {
        base.photonView.RPC("changeTitanPt", PhotonTargets.Others, titanPt);
        base.photonView.RPC("changeHumanPt", PhotonTargets.Others, humanPt);
    }

    public string GetState()
    {
        switch (state)
        {
            case CheckPointState.Human:
                return "[" + ColorSet.Human + "]H[-]";
            case CheckPointState.Titan:
                return "[" + ColorSet.TitanPlayer + "]T[-]";
            default:
                return "[FFFFFF]_[-]";
        }
    }

    private bool HasTeamWon(CheckPointState state)
    {
        foreach (PVPcheckPoint checkPoint in chkPts)
        {
            if (checkPoint.state != state)
            {
                return false;
            }
        }

        return true;
    }

    [RPC]
    private void changeHumanPt(float points)
    {
        humanPt = points;
    }

    [RPC]
    private void changeTitanPt(float points)
    {
        titanPt = points;
    }

    [RPC]
    private void changeState(int num)
    {
        state = (CheckPointState)num;
    }
}

using UnityEngine;

public class User : MonoBehaviour
{
    public enum UserState
    {
        NOTREADY,
        READY,
        PLAYING,
    }
    public UserState userState = UserState.NOTREADY;
    public int userid;
    public bool isLocalPlayer = true;

    public Pawn alpha;
    public Pawn bravo;
    public Pawn charlie;
    
    public Pawn cockpit;
    
    Vector3 spawnPosition = Vector3.zero;
    Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    
    public delegate bool InputDelegate(InputCtrl.InputParams inputs);
    public InputDelegate ProcessInput;
    
    void Awake()
    {
         GotoState(UserState.NOTREADY);
    }
    
    public Vector3 GetSpawnPosition()
    {
        Vector3 result = spawnPosition;
        spawnPosition += Vector3.right*5f;
        return result;
    }
    
    public Quaternion GetSpawnRotation()
    {
        return spawnRotation;
    }
    
    public void StartGame()
    {
        CamCtrl.Ins.pointsOfInterest.Clear();
        alpha = ActorWorld.Ins.RequestPawn("PawnA",this, Pawn.PawnType.SHIELD);
        if(isLocalPlayer)
        {
            CamCtrl.Ins.pointsOfInterest.Add(alpha.transform);
        }
        bravo = ActorWorld.Ins.RequestPawn("PawnB",this, Pawn.PawnType.TOOL);
        if(isLocalPlayer)
        {
            CamCtrl.Ins.pointsOfInterest.Add(bravo.transform);
        }
        charlie = ActorWorld.Ins.RequestPawn("PawnC",this, Pawn.PawnType.CANNON);
        if(isLocalPlayer)
        {
            CamCtrl.Ins.pointsOfInterest.Add(charlie.transform);
        }
        GotoState(UserState.PLAYING);
    }
    
    void GotoState(UserState state)
    {
        userState = state;
        switch(state)
        {
            case UserState.NOTREADY: 
                ProcessInput = NotReadyInput; 
                break;
            case UserState.READY:
                ProcessInput = ReadyInput; 
                StartGame(); 
                break;
            case UserState.PLAYING:
                ProcessInput = PlayingInput; 
                break;
        }
    }
    
    public bool NotReadyInput(InputCtrl.InputParams inputs)
    {
        if( inputs.primaryButton )
        {
            GotoState(UserState.READY);
        }
        return true;
    }
    
    public bool ReadyInput(InputCtrl.InputParams inputs)
    {
        return true;
    }
    
    public bool PlayingInput(InputCtrl.InputParams inputs)
    {
        if( cockpit != null )
        {
            cockpit.SetAxis(inputs.leftAxis);
            cockpit.DoInput(inputs.primaryButton, inputs.secondaryButton);
        }
        if( inputs.tertiaryButton )
        {
            if( cockpit == null )
            {
                cockpit = alpha;
            }
            else if( cockpit == alpha )
            {
                cockpit = bravo;
            }
            else if( cockpit == bravo )
            {
                cockpit = charlie;
            }
            else if( cockpit == charlie )
            {
                cockpit = alpha;
            }
            CamCtrl.Ins.keyInterest = cockpit != null ? cockpit.transform : null;
        }
        return true;
    }
}

using Komodo.Runtime;
using Komodo.Utilities;
using Unity.Entities;

//namespace Komodo.Runtime
//{
public class EraseManager : SingletonComponent<EraseManager>
{
    public static EraseManager Instance
    {
        get { return ((EraseManager)_Instance); }
        set { _Instance = value; }
    }

    TriggerEraseDraw leftHandErase;

    TriggerEraseDraw rightHandErase;

    public EntityManager entityManager;

    public virtual void Start()
    {
        //   entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    /// <summary>
    /// Funcion available to overide to include more networkobjects that can be erased and undone.
    /// </summary>
    /// <param name="netObj"></param>
    public virtual void TryAndErase(NetworkedGameObject netObj)
    {
        // var entityID = entityManager.GetComponentData<NetworkEntityIdentificationComponentData>(netObj.Entity).entityID;

        //draw functionality 
        // if (entityManager.HasComponent<DrawingTag>(netObj.Entity))
        // {

        switch (netObj.thisModelType)
        {
            case MODEL_TYPE.Drawing:
                netObj.gameObject.SetActive(false);

                //  when actions of erasing are being captured, the curStrokepos and curColor will both be set to 0.
                DrawingInstanceManager.Instance.SendDrawUpdate(netObj.thisEntityID, Entity_Type.LineNotRender);

                //save our reverted action for undoing the process with the undo button
                if (UndoRedoManager.IsAlive)
                {
                    UndoRedoManager.Instance.savedStrokeActions.Push
                    (
                            () =>
                            {
                                netObj.gameObject.SetActive(true);

                                DrawingInstanceManager.Instance.SendDrawUpdate(netObj.thisEntityID, Entity_Type.LineRender);
                            }
                    );
                }


                break;

            case MODEL_TYPE.Primitive:

                netObj.gameObject.SetActive(false);

                // tell other clients to do the same thing
                CreatePrimitiveManager.Instance.SendPrimitiveUpdate(netObj.thisEntityID, -9);

                // save to actions stack
                if (UndoRedoManager.IsAlive)
                {
                    UndoRedoManager.Instance.savedStrokeActions.Push(() =>
                    {
                        netObj.gameObject.SetActive(true);

                        CreatePrimitiveManager.Instance.SendPrimitiveUpdate(netObj.thisEntityID, 9);
                    });
                }
                break;



        }

        //if (netObj.thisModelType != MODEL_TYPE.Drawing)
        //    return;

        //turn it off for ourselves and others
        //netObj.gameObject.SetActive(false);

        //// when actions of erasing are being captured, the curStrokepos and curColor will both be set to 0. 
        //// DrawingInstanceManager.Instance.SendDrawUpdate(entityID, Entity_Type.LineNotRender);

        ////save our reverted action for undoing the process with the undo button
        //if (UndoRedoManager.IsAlive)
        //{
        //    UndoRedoManager.Instance.savedStrokeActions.Push
        //    (
        //        (System.Action)
        //        (
        //            () =>
        //            {
        //                netObj.gameObject.SetActive(true);

        //                DrawingInstanceManager.Instance.SendDrawUpdate(netObj.thisEntityID, Entity_Type.LineRender);
        //            }
        //        )
        //    );
        //}
        //}

    }
}
//}

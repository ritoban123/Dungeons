using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GuardActions
{
    public const int PAWN_LAYER_NUMBER = 8;

    public static void BasicGuardUpdate(Guard g, float deltaTime)
    {
        g.CurrentState.UpdateAction(g, deltaTime);
    }

    public static void BasicPatrolStateUpdate(Guard g, float deltaTime)
    {
        GuardController.instance.GetGameObjectForGuard(g).GetComponent<SpriteRenderer>().color = Color.red;
        g.Position = Vector2.MoveTowards(g.Position, g.NextTargetPosition, deltaTime * g.Data.MovementSpeed);
        if ((g.Position - g.NextTargetPosition).magnitude <= deltaTime * 0.4)
        {
            //Debug.Log((g.Position - g.NextTargetPosition).magnitude);
            g.AdvancePath(g.MoveToNextWaypoint);
        }
        //Debug.Log(g.NextTargetPosition);

        // Check if there are any pawns nearby (they have colliders to check for raycasts)
        Collider2D c = Physics2D.OverlapCircle(g.Position, g.Data.alertRadius, PawnLayerMask);
        if (c != null && PawnController.instance.IsGameObjectPawn(c.gameObject))
        {
            g.AlertedPawn = null; // EXTRA: This should already be set when we enter patrol state from alert or chase states, but it never hurts to make sure
            g.CurrentState.ChangeState(GuardState.AlertState);
        }
    }

    public static int PawnLayerMask = (1 << PAWN_LAYER_NUMBER);

    public static void BasicAlertStateUpdate(Guard g, float deltaTime)
    {
        GuardController.instance.GetGameObjectForGuard(g).GetComponent<SpriteRenderer>().color = Color.green;

        // Create a delay before we enter chase state. 
        // TODO: Play turning animation during this stage. Maybe perform line of sight every frame instead? And stop when you see something?
        // NOTE: Actions may not work when we move this to lua (it will probably be replaced by a string)
        Action expire = null;
        // TEMP: Jujst to see that a guard has entered alert state
        //sGuardController.instance.GetGameObjectForGuard(g).GetComponent<SpriteRenderer>().color = Color.green;
        expire = () =>
        {
            if (g.AlertedPawn == null)
            {
                // This is the first time we are running the alert state function
                Collider2D c = Physics2D.OverlapCircle(g.Position, g.Data.alertRadius, PawnLayerMask);
                if (c != null && PawnController.instance.IsGameObjectPawn(c.gameObject))
                {
                    g.AlertedPawn = PawnController.instance.GetPawn(c.gameObject);

                    g.CurrentState.ChangeState(GuardState.ChaseState);
                    //g.SetParameter("lastAlertedPawnX", g.AlertedPawn.FinalTargetPosition.y);
                    //g.SetParameter("lastAlertedPawnY", g.AlertedPawn.FinalTargetPosition.y);

                    // NOTE: Getting the dungeon directly from the DungeonController would have been easier, but this might work better if at some point Tile does not inherit from IPath_Node
                    g.ChangeDestination(PathfindingController.instance.tileGraph.dungeon.GetTileAt(Mathf.RoundToInt(g.AlertedPawn.FinalTargetPosition.x), Mathf.RoundToInt(g.AlertedPawn.FinalTargetPosition.y)));

                }
                else
                {
                    // A pawn was not found. Go back to patrol state
                    //GuardController.instance.GetGameObjectForGuard(g).GetComponent<SpriteRenderer>().color = Color.red;
                    g.CurrentState.ChangeState(GuardState.PatrolState);
                }
                //c.GetComponent<SpriteRenderer>().color = Color.green;
            }
        };
        if (GameManager.instance == null)
        {
            Debug.Log("GameManager.instance is null");
        }
        GameManager.instance.SetupTimer(g.Data.alertTime, null, expire);
    }
    public static void BasicChaseStateUpdate(Guard g, float deltaTime)
    {
        GuardController.instance.GetGameObjectForGuard(g).GetComponent<SpriteRenderer>().color = Color.blue;

        //Debug.Log("Chase" + g.Position);
        //GuardController.instance.GetGameObjectForGuard(g).GetComponent<SpriteRenderer>().color = Color.blue;

        g.Position = Vector2.MoveTowards(g.Position, g.NextTargetPosition, deltaTime * g.Data.MovementSpeed);


        //if (Mathf.Abs((g.GetParameter("lastAlertedPawnX") - g.AlertedPawn.FinalTargetPosition.x)) > 0.1 || Mathf.Abs(g.GetParameter("lastAlertedPawnY") - g.AlertedPawn.FinalTargetPosition.y) > 0.1)
        //{
        //    g.ChangeDestination(PathfindingController.instance.tileGraph.dungeon.GetTileAt(Mathf.RoundToInt(g.AlertedPawn.FinalTargetPosition.x), Mathf.RoundToInt(g.AlertedPawn.FinalTargetPosition.y)));
        //}

        if(g.aStar.endNode != g.AlertedPawn.aStar.endNode)
        {
            g.ChangeDestination(g.AlertedPawn.aStar.endNode);
        }

        //Debug.Log(Vector2.SqrMagnitude(g.Position - g.AlertedPawn.Position));
        if(Vector2.SqrMagnitude(g.Position - g.AlertedPawn.Position) >= (g.Data.maxChaseRadius * g.Data.maxChaseRadius))
        {
            // BUG: Once they return to patrol sate, they immediately return back to alert state
            //GuardController.instance.GetGameObjectForGuard(g).GetComponent<SpriteRenderer>().color = Color.red;

            g.CurrentState.ChangeState(GuardState.PatrolState);
        }

        if ((g.Position - g.NextTargetPosition).magnitude <= deltaTime * 0.4)
        {
            //Debug.Log((g.Position - g.NextTargetPosition).magnitude);
            // TODO: Enter fighting state, take away health
            // BUG: We enter patrol state again, and immediately go back to alert state
            //GuardController.instance.GetGameObjectForGuard(g).GetComponent<SpriteRenderer>().color = Color.red;

            g.AdvancePath(() => { g.CurrentState.ChangeState(GuardState.PatrolState); });
        }
    }

}

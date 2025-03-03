using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game
{
    public class InputManager : MonoBehaviour
    {
        private Camera mainCamera;
        private GroupDetector _groupDetector;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                GridBuilder.Instance.BuildGrid(LevelManager.Instance.CurrentLevelData);
            }
            
            if(Input.GetKeyDown(KeyCode.F))
            {
                GroupDetector.Instance.UpdateAllGroups();
            }
            
            if(Input.GetKeyDown(KeyCode.S))
                ShuffleHandler.Instance.ShuffleGrid();

            if (Input.GetMouseButtonDown(0))
            {
                HandleInput();
            }
        }
        private void HandleInput()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                BlockBase block = hit.collider.GetComponent<BlockBase>();
                if(!block.CanBeClicked)
                    return;
                if (block && block is RegularBlock)
                {

                    // Use GroupDetection to find all connected blocks
                    List<BlockBase> group = GroupDetector.Instance.DetectGroup(block);

                    if (group.Count > LevelManager.Instance.CurrentLevelData.A_firstThreshold-1) 
                    {
                        BlastHandler.Instance.HandleBlast(group);
                    }
                    else
                    {
                        Debug.Log("No group to blast.");
                    }
                }
            }
        }

//         private void HandleInput()
//         {
//             Debug.Log("Mouse clicked!");
//             Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
//             RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
//             if (hit.collider != null)
//             {
//                 Debug.Log(hit.collider.name);
//                 BlockBase block = hit.collider.GetComponent<BlockBase>();
//                 if (block)
//                 {
//                     Debug.Log("Block clicked!");
//                     Debug.Log("Clicked block position = "+block.Position);
//                     BlastHandler.Instance.HandleBlast(block);
//                 }
//             }
// }
//             // if (Physics2D.Raycast(ray.origin, out RaycastHit hit))
//             // {
//             //     Debug.Log(hit.collider.name);
//             //     BlockBase block = hit.collider.GetComponent<BlockBase>();
//             //     if (block)
//             //     {
//             //         blastHandler.HandleBlast(block);
//             //     }
//             // }
//             
//             
//             
//         }
    }
}
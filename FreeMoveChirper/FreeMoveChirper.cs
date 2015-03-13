using System;
using UnityEngine;
using ColossalFramework;
using ICities;

namespace FreeMoveChirper
{
    public class FreeMoveChirp : IUserMod
    {
        public string Name
        {
            get { return "Chirper Position Changer 1.1"; }
        }

        public string Description
        {
            get { return "Click and drag the chirper wherever you want it to be"; }
        }
    }

    public class Chirp : ChirperExtensionBase
    {

        private IChirper currentChirper;

        private Vector2 defaultChirperPos;
        private Vector2 mousePosOnClick;
        
        private ChirperAnchor currentAnchor;

        private bool clickedOnChirp = false;

        public override void OnCreated(IChirper c)
        {
            currentChirper = c;
            currentChirper.SetBuiltinChirperFree(true);
            defaultChirperPos = currentChirper.builtinChirperPosition;
            currentAnchor = ChirperAnchor.TopCenter;
            currentChirper.SetBuiltinChirperAnchor(currentAnchor);
        }

        public override void OnUpdate()
        {
            //Test if player clicked on chirp
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                ChirpMessage.SendMessage(defaultChirperPos.ToString());

                if (IsClickNearChirp(GetMousePos(), currentChirper.builtinChirperPosition))
                    clickedOnChirp = true;

                mousePosOnClick = GetMousePos();
            }

            //Has the player released the mouse button?
            if (Input.GetMouseButtonUp(0) && clickedOnChirp)
            {
                clickedOnChirp = false;
            }

            //Move chirper
            if (Input.GetMouseButton(0) && clickedOnChirp)
            {
                float minDistance = 15.0f; //Minimum distance the user need to drag the mouse for the chirper to move
                float distanceMoved = ((GetMousePos()) - mousePosOnClick).magnitude;
                bool movedFarEnough = distanceMoved > minDistance;

                if (movedFarEnough)
                {
                    currentChirper.builtinChirperPosition = GetMousePos();
                }
            }

            //Change chirper Anchor on rightclick
            if (Input.GetMouseButtonUp(1))
            {
                
            }

            //Reset chirper position on key combination
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.O))
            {
                currentChirper.builtinChirperPosition = defaultChirperPos;
            }
        }

        private bool IsClickNearChirp(Vector2 mousePosOnClick, Vector2 chirperPosOnClick)
        {
            float deltaDistance = 50.0f; //Maximum distance a click is registered on the chirper

            if ((chirperPosOnClick - mousePosOnClick).magnitude < deltaDistance)
                return true;
            else
                return false;
        }

        private Vector2 GetMousePos()
        {
            Vector2 mousePos = Event.current.mousePosition;

            //No matter what resolution the user has set the GUI will always 
            //have this resolution (May change in future)
            float GUIWidth = 1920f;
            float GUIHeight = 1080f;

            mousePos.x *= (GUIWidth / (float)Screen.width);
            mousePos.y *= (GUIHeight / (float)Screen.height);

            return mousePos;
        }
    }


}

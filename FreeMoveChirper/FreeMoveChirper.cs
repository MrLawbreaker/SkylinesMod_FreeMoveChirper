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
            get { return "Chirper Position Changer 1.2"; }
        }

        public string Description
        {
            get { return "Click and drag the chirper wherever you want it to be"; }
        }
    }

    public class Chirp : ChirperExtensionBase
    {

        private IChirper currentChirper;
        private Vector2 currentChirperPos;

        private Vector2 defaultChirperPos;
        private Vector2 mousePosOnClick;

        //No matter what resolution the user has set the GUI will always 
        //have this resolution (May change in future)
        private float GUIWidth = 1920f;
        private float GUIHeight = 1080f;

        private bool leftClickedOnChirp = false;
        private bool wasMoved = false;

        public override void OnCreated(IChirper c)
        {
            currentChirper = c;
            currentChirper.SetBuiltinChirperFree(true);
            defaultChirperPos = currentChirper.builtinChirperPosition;
            currentChirperPos = currentChirper.builtinChirperPosition;
            UpdateAnchor();
        }

        public override void OnUpdate()
        {
            //Test if player clicked on chirp
            if (Input.GetMouseButtonDown(0))
            {
                if (IsClickNearChirp(GetMousePos(), currentChirper.builtinChirperPosition))
                {
                        leftClickedOnChirp = Input.GetMouseButtonDown(0);
                }

                mousePosOnClick = GetMousePos();
            }


            //Move chirper
            if (Input.GetMouseButton(0) && leftClickedOnChirp)
            {
                float minDistance = 15.0f; //Minimum distance the user need to drag the mouse for the chirper to move
                float distanceMoved = ((GetMousePos()) - mousePosOnClick).magnitude;
                bool movedFarEnough = distanceMoved > minDistance;

                if (movedFarEnough)
                {
                    ChirpPanel.instance.Collapse();
                    wasMoved = true;
                    SetChirpPosition(GetMousePos(), false);
                }
            }
            
            

            //Has the player released the mouse button?
            if (Input.GetMouseButtonUp(0) && leftClickedOnChirp && wasMoved)
            {
                leftClickedOnChirp = false;
                wasMoved = false;

                ChirpPanel.instance.Collapse();

                UpdateAnchor();
            }

            //Reset chirper position on key combination
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O))
            {
                SetChirpPosition(defaultChirperPos, true);
            }

            currentChirperPos = currentChirper.builtinChirperPosition;
        }

        private void SetChirpPosition(Vector2 position, bool updateAnchor)
        {
            currentChirper.builtinChirperPosition = position;

            currentChirperPos = position;
            if(updateAnchor)
                UpdateAnchor();
        }


        private void UpdateAnchor()
        {
            ChirperAnchor currentAnchor;
            Vector2 currentPos = currentChirper.builtinChirperPosition;

            float thirdOfScreenWidth = GUIWidth / 3;
            float halfOfScreenHeight = GUIHeight / 2;

            if (currentPos.y <= halfOfScreenHeight)
            {
                //Chirper is at the upper half of the screen
                if (currentPos.x <= thirdOfScreenWidth)
                {
                    //Chirper is at the left side of the screen
                    currentAnchor = ChirperAnchor.TopLeft;
                }
                else if (currentPos.x <= (thirdOfScreenWidth) * 2)
                {
                    //Chirper is at the middle of the screen
                    currentAnchor = ChirperAnchor.TopCenter;
                }
                else
                {
                    //Chirper is at the right side of the screen
                    currentAnchor = ChirperAnchor.TopRight;
                }
            }
            else {
                //Chirper is at the lower half of the screen
                if (currentPos.x <= thirdOfScreenWidth)
                {
                    //Chirper is at the left side of the screen
                    currentAnchor = ChirperAnchor.BottomLeft;
                }
                else if (currentPos.x <= (thirdOfScreenWidth) * 2)
                {
                    //Chirper is at the middle of the screen
                    currentAnchor = ChirperAnchor.BottomCenter;
                }
                else
                {
                    //Chirper is at the right side of the screen
                    currentAnchor = ChirperAnchor.BottomRight;
                }
            }

            currentChirper.SetBuiltinChirperAnchor(currentAnchor);
        }

        private bool IsClickNearChirp(Vector2 mousePosOnClick, Vector2 chirperPosOnClick)
        {
            float deltaDistance = 32.0f; //Maximum distance a click is registered on the chirper

            if ((chirperPosOnClick - mousePosOnClick).magnitude < deltaDistance)
                return true;
            else
                return false;
        }

        private Vector2 GetMousePos()
        {
            Vector2 mousePos = Event.current.mousePosition;

            mousePos.x *= (GUIWidth / Screen.width);
            mousePos.y *= (GUIHeight / Screen.height);

            return mousePos;
        }
    }
}

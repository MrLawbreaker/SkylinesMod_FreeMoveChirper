using System;
using UnityEngine;
using ColossalFramework;
using ICities;
using ColossalFramework.Plugins;

namespace FreeMoveChirper
{
    public class FreeMoveChirp : IUserMod
    {
        public string Name
        {
            get { return "Chirper Position Changer 1.3.1"; }
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

        //The GUI Resolution is dependent from the aspect ratio the user has set
        private float GUIWidth = float.NaN;
        private float GUIHeight = 1080f;

        private int ScreenWidth;
        private int ScreenHeight;

        private bool resSupported = true;
        private bool leftClickedOnChirp = false;
        private bool wasMoved = false;

        public override void OnCreated(IChirper c)
        {
            //Init
            SetScreenRes();
            SetGUISize();
            ChirpPanel.instance.component.BringToFront();
            currentChirper = c;
            currentChirper.SetBuiltinChirperFree(true);
            defaultChirperPos = currentChirper.builtinChirperPosition;
            currentChirperPos = currentChirper.builtinChirperPosition;
            UpdateAnchor();
        }

        private void SetScreenRes()
        {
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;
        }

        private void SetGUISize()
        {
            float ratio = (float)ScreenWidth / (float)ScreenHeight;
            if (ratio > 1.9)
            {
                resSupported = false;
            }
            else if (ratio >= 1.7)
            {
                //16:9
                GUIWidth = 1920f;
            }
            else if (ratio >= 1.6)
            {
                //16:10
                GUIWidth = 1730f;
            }
            else if (ratio >= 1.3f)
            {
                //4:3
                GUIWidth = 1430f;
            }
            else
            {
                resSupported = false;
            }
        }

        public override void OnUpdate()
        {

            
            //Check for change in Resolution size
            if (Screen.width != ScreenWidth || Screen.height != ScreenHeight)
            {
                SetScreenRes();
                SetGUISize();
            }
            
            //Check if Screen Resolution is supported
            if (!resSupported)
            {
                if (float.IsNaN(GUIWidth))
                {
                    ChirpMessage.SendMessage("FreeMoveChirperMod", "I am sorry but your resolution is not supported");
                    GUIWidth = 0f;
                }
                return;
            }

            //Test if player clicked on chirp
            if (Input.GetMouseButtonDown(0))
            {
                if (IsClickNearChirp(GetMousePos(), currentChirper.builtinChirperPosition))
                {
                        leftClickedOnChirp = Input.GetMouseButtonDown(0);
                }

                mousePosOnClick = GetMousePos();
            }

            //Has the player released the mouse button?
            if (Input.GetMouseButtonUp(0) && leftClickedOnChirp)
            {
                leftClickedOnChirp = false;

                if (wasMoved)
                {
                    ChirpPanel.instance.Collapse();
                }
                wasMoved = false;

                UpdateAnchor();
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

            mousePos.x *= (GUIWidth / (float)ScreenWidth);
            mousePos.y *= (GUIHeight / (float)ScreenHeight);

            return mousePos;
        }
    }
}

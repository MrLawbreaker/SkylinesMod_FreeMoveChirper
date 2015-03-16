using System;
using UnityEngine;
using ColossalFramework;
using ICities;
using ColossalFramework.Plugins;
using ColossalFramework.UI;

namespace FreeMoveChirper
{
    public class FreeMoveChirp : IUserMod
    {
        public string Name
        {
            get { return "Chirper Position Changer 1.5"; }
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
        private float GUIWidth;
        private float GUIHeight;

        private int ScreenWidth;
        private int ScreenHeight;

        private bool leftClickedOnChirp = false;
        private bool wasMoved = false;

        private Camera currentCam;
        private UIView currentUIView;

        public override void OnCreated(IChirper c)
        {
            //Init
            currentUIView = ChirpPanel.instance.component.GetUIView();
            currentCam = currentUIView.uiCamera;

            ChirpPanel.instance.component.BringToFront();
            currentChirper = c;
            currentChirper.SetBuiltinChirperFree(true);
            currentChirperPos = currentChirper.builtinChirperPosition;
            defaultChirperPos = currentChirper.builtinChirperPosition;

            SetScreenRes();
            SetGUISize();
            if (!LoadSavedPosition())
                UpdateAnchor();
        }

        public override void OnUpdate()
        {
            //Check for change in Resolution size
            if (Screen.width != ScreenWidth || Screen.height != ScreenHeight)
            {
                SetScreenRes();
                SetGUISize();
            }
            
            //Test if player clicked on chirp
            if (Input.GetMouseButtonDown(0))
            {
                if (IsClickNearChirp())
                {
                        leftClickedOnChirp = true;
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
                    wasMoved = false;
                    UpdateAnchor();

                    SavePosition();
                }
            }

            //Move chirper
            if (Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && leftClickedOnChirp)
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
                ResetPosition();
            }

            currentChirperPos = currentChirper.builtinChirperPosition;
        }

        private void SetScreenRes()
        {
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;
        }

        private void SetGUISize()
        {
            GUIWidth = currentUIView.GetScreenResolution().x;
            GUIHeight = currentUIView.GetScreenResolution().y;
        }

        private bool LoadSavedPosition()
        {
            if (Properties.Settings.Default.position_x > 0 && Properties.Settings.Default.position_y > 0)
            {
                Vector2 position = new Vector2();
                position.x = Properties.Settings.Default.position_x * GUIWidth;
                position.y = Properties.Settings.Default.position_y * GUIHeight;
                SetChirpPosition(position);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ResetPosition()
        {
            SetChirpPosition(defaultChirperPos);
        }

        private void SavePosition()
        {
            Vector2 position = currentChirper.builtinChirperPosition;
            position.x /= GUIWidth;
            position.y /= GUIHeight;

            Properties.Settings.Default.position_x = position.x;
            Properties.Settings.Default.position_y = position.y;

            Properties.Settings.Default.Save();
        }

        private void SetChirpPosition(Vector2 position, bool updateAnchor = true)
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

        private bool IsClickNearChirp()
        {
            return (GetMousePos() - currentChirper.builtinChirperPosition).magnitude < 35f;
        }

        private Vector2 GetMousePos()
        {
            Vector3 mouseWorldPos = Camera.current.ScreenToWorldPoint(Input.mousePosition);
            return currentUIView.WorldPointToGUI(currentCam, mouseWorldPos);
        }
    }
}

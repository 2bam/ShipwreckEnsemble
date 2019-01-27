using UnityEngine;

public class TouchController : MonoBehaviour
{
    [SerializeField] private bool _touchTap = true;

    [SerializeField] private PlayerController _player;

    [SerializeField] private float TapMaxTime = 0.5f;

    private float _touchTime = 0;
    private bool _isTouch0;

    private Vector3 _startTouchPos;

    void Start()
    {
    }

    void Update()
    {
        if (_isTouch0)
        {
            _touchTime += Time.deltaTime;
        }

        if (_touchTap)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {

                    _startTouchPos = Input.GetTouch(0).position;
                    _touchTime = 0;
                    _isTouch0 = true;

                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (_touchTime < TapMaxTime&&Vector3.Distance(_startTouchPos, Input.GetTouch(0).position) <25f)
                    {
                        if (Input.GetTouch(0).position.x > Screen.width / 2)
                        {
                            _player.RotateTo(true);
                        }
                        else if (Input.GetTouch(0).position.x < Screen.width / 2)
                        {
                            _player.RotateTo(false);
                        }
                    }
                    _isTouch0 = false;
                }
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (Input.mousePosition.x > Screen.width / 2 /*&& Input.mousePosition.y > Screen.height / 5*/)
        //    {
        //        _player.RotateTo(true);
        //    }
        //    else if (Input.mousePosition.x < Screen.width / 2/* && Input.mousePosition.y > Screen.height / 5*/)
        //    {
        //        _player.RotateTo(false);
        //    }
        //}

    }
}

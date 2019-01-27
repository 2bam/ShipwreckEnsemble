using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool rotationActive = false;

    public void RotateTo(bool direction)
    {

        if (direction)
        {
            StartCoroutine(ROTATION_CO(1));
        }
        else
        {
            StartCoroutine(ROTATION_CO(-1));
        }
    }

    IEnumerator ROTATION_CO(float direction)
    {
        if (rotationActive) yield break;

        AudioManager.Instance.PlayerRotation();

        rotationActive = true;

        float myangle = Mathf.Abs(transform.eulerAngles.z % 360);
        float targetangle = 0;

        if (direction < 0)
        {
            targetangle = myangle + 90;
        }
        else
        {
            targetangle = myangle - 90;
        }

        Quaternion myrot = Quaternion.Euler(0, 0, targetangle);
        float timecount = 0;

        while (timecount < 0.5f)
        {
            timecount += Time.deltaTime;


            transform.rotation = Quaternion.Slerp(transform.rotation, myrot, Time.deltaTime * 7f);

            yield return null;
        }
        //normaliza

        myangle = Mathf.Abs(transform.eulerAngles.z % 360);
        if ((myangle > 45 && myangle <= 135))
            transform.eulerAngles = new Vector3(0, 0, 90);
        else if ((myangle > 135 && myangle <= 225))
            transform.eulerAngles = new Vector3(0, 0, 180);
        else if ((myangle > 225 && myangle <= 315))
            transform.eulerAngles = new Vector3(0, 0, 270);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);

        rotationActive = false;
        //Body.Rotate(Vector3.back / 2 * _Rb.velocity.y * Time.deltaTime);

    }
}

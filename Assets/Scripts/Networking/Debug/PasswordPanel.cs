using UnityEngine;
using System.Collections;

public class PasswordPanel : MonoBehaviour {
    private string password;
    private UdpKit.UdpSession session;

    public void Cancel() {
        gameObject.SetActive(false);
    }

    public void Pop(UdpKit.UdpSession session) {
        this.session = session;
        this.password = "";
        gameObject.SetActive(true);
    }

    public void Connect() {
        BoltNetwork.Connect(session, new ConnectionRequestData(GameManager.instance.CurrentUserName, password));
        gameObject.SetActive(false);
    }

    public void UpdatePassword(string newPassword) {
        this.password = newPassword;
    }
}

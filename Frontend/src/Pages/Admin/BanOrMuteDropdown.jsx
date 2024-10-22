/* eslint-disable react/prop-types */
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faCheck} from "@fortawesome/free-solid-svg-icons";
import {toast} from "react-hot-toast";
import {useState} from "react";
import {useCookies} from "react-cookie";

export const BanOrMuteDropdown = ({
                                      banOrMuteOptionsShown,
                                      setBanOrMuteOptionsShown,
                                      selected,
                                      setSelected,
                                      username
                                  }) => {
    const showErrorToast = (message) => toast.error(message);
    const [muteFor, setMuteFor] = useState(0);
    const [cookies] = useCookies(['user']);

    const BanOrMuteUser = async () => {
        if (selected === "Ban") {
            await BanUser();
            setBanOrMuteOptionsShown(() => false);
            setSelected(() => null);
            return;
        }
        await MuteUser();
        setBanOrMuteOptionsShown(() => false);
        setSelected(() => null);
    }


    const BanUser = async () => {
        const res = await fetch("/api/admin/users/ban/" + username, {
            method: "PATCH",
            headers: {
                "Authorization": "Bearer " + cookies.user
            }
        });
        const data = await res.json();
        if (data.message) {
            showErrorToast(data.message);
            return;
        }
        toast.success("Successfully banned " + username);
    }

    const MuteUser = async () => {
        const res = await fetch("/api/admin/users/mute/" + username, {
            method: "PATCH",
            headers: {
                "Authorization": "Bearer " + cookies.user,
                "Content-Type": "application/json",
            },
            body: JSON.stringify({Time: muteFor})
        });
        console.log(JSON.stringify({Time: muteFor}));
        if (res.status !== 200) {
            showErrorToast("Something went wrong!");
            return;
        }
        const data = await res.json();
        if (data.message) {
            showErrorToast(data.message);
            return;
        }
        console.log(muteFor)
        toast.success("Successfully muted " + username + " for " + muteFor + " hour" + (muteFor === 1 ? "" : "s"));
    }


    return <>{banOrMuteOptionsShown ? <><select className="border-2 rounded-md" onChange={(event) => {
        setSelected(() => event.target.value === "Option" ? null : event.target.value)
    }}>
        <option selected={selected === null}>Option</option>
        <option>Ban</option>
        <option>Mute</option>
    </select>{selected === "Mute" ?
        <input onChange={(event) => setMuteFor(() => parseInt(event.target.value))} type="number"
               className=" ml-2 w-3/5 border-2 rounded-md" placeholder="mute for n hours"/> : <></>}
        {selected ?
            <button title="confirm" onClick={async () => {
                await BanOrMuteUser()
            }} className="ml-2"><FontAwesomeIcon
                className="text-gray-300 hover:text-green-500 transition"
                icon={faCheck}/>
            </button> : <></>}</> : <></>}</>

}
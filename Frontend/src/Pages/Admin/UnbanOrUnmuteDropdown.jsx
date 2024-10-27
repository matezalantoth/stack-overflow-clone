import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faCheck} from "@fortawesome/free-solid-svg-icons";
import {toast} from "react-hot-toast";
import {useCookies} from "react-cookie";

export const UnbanOrUnmuteDropdown = ({
                                          unbanOrUnmuteOptionsShown,
                                          setUnBanOrUnMuteOptionsShown,
                                          selected,
                                          setSelected,
                                          username
                                      }) => {

    const [cookies] = useCookies(['user']);
    const showErrorToast = (message) => toast.error(message);

    const UnbanOrUnmuteUser = async () => {
        if (selected === "Unban") {
            await UnbanUser();
            setUnBanOrUnMuteOptionsShown(() => false);
            setSelected(() => null);
            return;
        }
        await UnmuteUser();
        setUnBanOrUnMuteOptionsShown(() => false);
        setSelected(() => null);
    }

    const UnbanUser = async () => {
        const res = await fetch("/api/admin/users/unban/" + username, {
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
        toast.success("Successfully unbanned " + username);
    }

    const UnmuteUser = async () => {
        const res = await fetch("/api/admin/users/unmute/" + username, {
            method: "PATCH",
            headers: {
                "Authorization": "Bearer " + cookies.user,
                "Content-Type": "application/json",
            }
        });
        if (res.status !== 200) {
            showErrorToast("Something went wrong!");
            return;
        }
        const data = await res.json();
        if (data.message) {
            showErrorToast(data.message);
            return;
        }
        toast.success("Successfully unmuted " + username);
    }
    console.log(unbanOrUnmuteOptionsShown);

    return <>{unbanOrUnmuteOptionsShown ? <><select className="border-2 rounded-md" onChange={(event) => {
        setSelected(() => event.target.value === "Option" ? null : event.target.value)
    }}>
        <option selected={selected === null}>Option</option>
        <option>Unban</option>
        <option>Unmute</option>
    </select>{selected ? <button title="confirm" onClick={async () => {
        await UnbanOrUnmuteUser()
    }} className="ml-2"><FontAwesomeIcon
        className="text-gray-300 hover:text-green-500 transition"
        icon={faCheck}/>
    </button> : <></>}</> : <></>}


    </>
}
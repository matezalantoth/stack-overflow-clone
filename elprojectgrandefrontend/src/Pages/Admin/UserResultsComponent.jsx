import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faBan, faGavel} from "@fortawesome/free-solid-svg-icons";
import {BanOrMuteDropdown} from "./BanOrMuteDropdown.jsx";
import {useEffect, useState} from "react";
import {UnbanOrUnmuteDropdown} from "./UnbanOrUnmuteDropdown.jsx";

export const UserResultsComponent = ({selected, setSelected, username}) => {
    const [banOrMuteOptionsShown, setBanOrMuteOptionsShown] = useState(false);
    const [unbanOrUnmuteOptionsShown, setUnbanOrUnmuteOptionsShown] = useState(false);

    useEffect(() => {
        if (banOrMuteOptionsShown) {
            setUnbanOrUnmuteOptionsShown(false);
        }
        if (unbanOrUnmuteOptionsShown) {
            setBanOrMuteOptionsShown(false);
        }

    }, [banOrMuteOptionsShown, unbanOrUnmuteOptionsShown]);


    return <>
        <button className="text-blue-400" title="Unban/Unmute"
                onClick={() => setUnbanOrUnmuteOptionsShown(!unbanOrUnmuteOptionsShown)}><FontAwesomeIcon
            icon={faGavel}/>
        </button>

        <button onClick={() => {
            setBanOrMuteOptionsShown(!banOrMuteOptionsShown)
        }} className="m-2 text-red-500" title="Ban/Mute"><FontAwesomeIcon icon={faBan}/></button>
        <BanOrMuteDropdown banOrMuteOptionsShown={banOrMuteOptionsShown}
                           setBanOrMuteOptionsShown={setBanOrMuteOptionsShown} selected={selected}
                           setSelected={setSelected} username={username}/>
        <UnbanOrUnmuteDropdown unbanOrUnmuteOptionsShown={unbanOrUnmuteOptionsShown}
                               setUnBanOrUnMuteOptionsShown={setUnbanOrUnmuteOptionsShown} selected={selected}
                               setSelected={setSelected} username={username}/>
    </>
}
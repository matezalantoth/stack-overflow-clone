import {faBan, faCheck, faFeather, faX} from "@fortawesome/free-solid-svg-icons";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {useState} from "react";

export default function ResultInteractionComponent({searchModel}) {
    const [banOrMuteOptionsShown, setBanOrMuteOptionsShown] = useState(false);
    const [selected, setSelected] = useState(null);
    const [muteFor, setMuteFor] = useState(0);
    switch (searchModel) {
        default:
            return (<>
                <button className="m-2 text-blue-400" title="Edit"><FontAwesomeIcon icon={faFeather}/></button>
                <button className="text-red-500" title="Delete"><FontAwesomeIcon icon={faX}/></button>
            </>)
        case 'Users':
            return (
                <>
                    <button onClick={() => {
                        setBanOrMuteOptionsShown(!setBanOrMuteOptionsShown())
                    }} className="m-2 text-amber-300" title="Ban/Mute"><FontAwesomeIcon icon={faBan}/></button>
                    <button className="text-red-500" title="Delete"><FontAwesomeIcon icon={faX}/></button>
                    {banOrMuteOptionsShown ? <select className="border-2 rounded-md" onChange={(event) => {
                        setSelected(() => event.target.value === "Option" ? null : event.target.value)
                    }}>
                        <option selected={selected === null}>Option</option>
                        <option>Ban</option>
                        <option>Mute</option>
                    </select> : <></>}
                    {selected === "Mute" ?
                        <input onChange={(event) => setMuteFor(() => event.target.value)} type="number"
                               className=" ml-2 w-3/5 border-2 rounded-md" placeholder="mute for n hours"/> : <></>}
                    {selected ?
                        <button title="confirm" className="ml-2"><FontAwesomeIcon
                            className="text-gray-300 hover:text-green-500 transition"
                            icon={faCheck}/>
                        </button> : <></>}
                </>
            )
    }
}
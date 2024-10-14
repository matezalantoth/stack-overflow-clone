import {faBan, faCheck, faFeather, faX} from "@fortawesome/free-solid-svg-icons";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {useState} from "react";
import {useNavigate} from "react-router-dom";
import {useCookies} from "react-cookie";

export default function ResultInteractionComponent({searchModel, id}) {
    const [banOrMuteOptionsShown, setBanOrMuteOptionsShown] = useState(false);
    const [selected, setSelected] = useState(null);
    const [muteFor, setMuteFor] = useState(0);
    const navigate = useNavigate();
    const [cookies] = useCookies(['user']);

    const deleteQuestion = async (id) => {
        const res = await fetch('/api/questions/' + id, {
            method: 'DELETE',
            headers: {
                'Authorization': "Bearer " + cookies.user
            }
        })
        const data = await res.json()
    }

    const deleteAnswer = async (id) => {
        await fetch("/api/answers/" + id, {
            method: "DELETE",
            headers: {
                "Authorization": "Bearer " + cookies.user
            }
        })
    }

    const getQuestionId = async () => {
        const res = await fetch('/api/answers/question/' + id, {
            method: 'GET',
            headers: {
                'Authorization': "Bearer " + cookies.user
            }
        })
        const data = await res.json();
        console.log(data);
        return data;
    }

    const navToQuestionPage = async () => {
        const qId = await getQuestionId();
        console.log(qId);
        navigate("/question/" + qId + "?state=ansEdit&id=" + id);
    }

    console.log(id);
    switch (searchModel) {
        default:
            return (<>
                <button className="m-2 text-blue-400" title="Edit" onClick={() => navToQuestionPage()}><FontAwesomeIcon
                    icon={faFeather}/></button>
                <button className="text-red-500" onClick={() => deleteAnswer(id)} title="Delete"><FontAwesomeIcon
                    icon={faX}/></button>
            </>)
        case 'Questions':
            return (<>
                <button className="m-2 text-blue-400" onClick={() => navigate("/question/" + id + "?state=edit")}
                        title="Edit">
                    <FontAwesomeIcon
                        icon={faFeather}/>
                </button>
                <button className="text-red-500" title="Delete" onClick={() => deleteQuestion(id)}><FontAwesomeIcon
                    icon={faX}/></button>
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
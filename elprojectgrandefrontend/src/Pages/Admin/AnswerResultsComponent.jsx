import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faFeather, faX} from "@fortawesome/free-solid-svg-icons";
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";
import {useNavigate} from "react-router-dom";

export const AnswerResultsComponent = ({id, setSearchResults}) => {

    const [cookies] = useCookies(['user']);
    const showErrorToast = (message) => toast.error(message);
    const navigate = useNavigate();

    const getQuestionId = async () => {
        const res = await fetch('/api/answers/question/' + id, {
            method: 'GET',
            headers: {
                'Authorization': "Bearer " + cookies.user
            }
        })
        const data = await res.json();
        if (data.message) {
            showErrorToast(data.message);
            return;
        }
        return data;
    }
    const navToQuestionPage = async () => {
        const qId = await getQuestionId();
        navigate("/question/" + qId + "?state=ansEdit&id=" + id);
    }


    const deleteAnswer = async (id) => {
        const res = await fetch("/api/answers/" + id, {
            method: "DELETE",
            headers: {
                "Authorization": "Bearer " + cookies.user
            }
        })
        try {
            const data = await res.json();
            if (data.message) {
                showErrorToast(data.message);

            }
        } catch (e) {
            console.error(e)
            console.log("setting results to exclude id: " + id);
            setSearchResults(prevRes => prevRes.filter(res => res.id !== id));
        }


    }

    return (<>
        <button className="m-2 text-blue-400" title="Edit" onClick={() => navToQuestionPage()}><FontAwesomeIcon
            icon={faFeather}/></button>
        <button className="text-red-500" onClick={() => deleteAnswer(id)} title="Delete"><FontAwesomeIcon
            icon={faX}/></button>
    </>)
}
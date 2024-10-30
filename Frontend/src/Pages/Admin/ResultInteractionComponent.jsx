import {useState} from "react";
import {UserResultsComponent} from "./UserResultsComponent.jsx";
import {QuestionResultsComponent} from "./QuestionResultsComponent.jsx";
import {AnswerResultsComponent} from "./AnswerResultsComponent.jsx";
import TagResultComponent from "./TagResultComponent.jsx";

export default function ResultInteractionComponent({searchModel, id, setSearchResults}) {

    const [selected, setSelected] = useState(null);

    const deleteTag = async (id) => {
        await fetch(`/api/tags/${id}`, {
            method: 'DELETE',
            headers: {
                "Authorization": "Bearer " + cookies.user
            }
        })
    }



    switch (searchModel) {
        default:
            return <AnswerResultsComponent id={id} setSearchResults={setSearchResults}/>
        case 'Questions':
            return <QuestionResultsComponent id={id} setSearchResults={setSearchResults}/>
        case 'Tags':
            return <TagResultComponent id={id} setSearchResults={setSearchResults}/>
        case 'Users':
            return <UserResultsComponent selected={selected} setSelected={setSelected} username={id}/>

    }
}
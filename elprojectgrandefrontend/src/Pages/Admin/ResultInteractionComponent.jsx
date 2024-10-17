import {useState} from "react";
import {UserResultsComponent} from "./UserResultsComponent.jsx";
import {QuestionResultsComponent} from "./QuestionResultsComponent.jsx";
import {AnswerResultsComponent} from "./AnswerResultsComponent.jsx";

export default function ResultInteractionComponent({searchModel, id, setSearchResults}) {

    const [selected, setSelected] = useState(null);


    switch (searchModel) {
        default:
            return <AnswerResultsComponent id={id} setSearchResults={setSearchResults}/>
        case 'Questions':
            return <QuestionResultsComponent id={id} setSearchResults={setSearchResults}/>
        case 'Users':
            return <UserResultsComponent selected={selected} setSelected={setSelected} username={id}/>

    }
}
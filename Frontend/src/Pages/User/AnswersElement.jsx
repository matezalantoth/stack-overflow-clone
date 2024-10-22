import {useNavigate} from "react-router-dom";

export default function AnswersElement({answers}) {
    const navigate = useNavigate()
    return (
        <div id="AnswerHolder">
            {answers.map(answer => {
                console.log(answer)
                return (<div>
                    <div className='hover:underline text-blue-600'
                         onClick={() => navigate('/question/' + answer.question.id)}>{answer.question.title}</div>
                    <div>{answer.content}</div>
                    <div>{answer.postedAt}</div>
                </div>)
            })}
        </div>
    )
}
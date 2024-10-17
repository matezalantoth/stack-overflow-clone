/* eslint-disable react/prop-types */
import Tag from "../../components/Tag/Tag.jsx";
import {useNavigate} from "react-router-dom";

export const QuestionsComponent = ({questions}) => {
    const navigate = useNavigate();
    return <div
        className="welcomePage text-center w-120 flex justify-center items-center flex-col mt-4">
        {questions.map((question) => {
            return (<div key={question.Id}
                         className='relative h-20 text-black border-b-2 border-l-2 rounded hover:bg-gray-100 transition w-120 overflow-hidden'
            >
                <h2 className='text-blue-700 cursor-pointer ml-2 break-words' onClick={() => {
                    navigate(`/question/${question.id}`);
                }}>{question.title}</h2>
                <p className='text-xs line-clamp-3 text-gray-600 leading-4 break-words ml-2'>{question.content}</p>
                <div className={"flex flex-row gap-3 justify-center"}>
                    {question.tags.map((tag) => {
                        return (<Tag key={tag} tag={tag}/>)
                    })}
                </div>
            </div>)
        })}
    </div>
}
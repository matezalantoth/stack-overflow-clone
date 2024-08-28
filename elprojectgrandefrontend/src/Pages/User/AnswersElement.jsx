export default function AnswersElement ({answers})
{
    return  (
        
        <div id= "AnswerHolder" >
            {answers.map(answer => {
                console.log(answer)
               return (<div><div>{answer.content}</div><div>{answer.postedAt}</div></div>)
            })}
        </div>
    )
}
import {useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";
import {useCookies} from "react-cookie";
import TrendingQuestions from "../../components/TrendingQuestions/TrendingQuestions.jsx";
import IndexQuestion from "../../components/IndexQuestion/IndexQuestion.jsx";
import Tag from "../../components/Tag/Tag.jsx";


export default function WelcomePage({searchQuestion, normalQuestion, setNormalQuestion, setsearchQuestion}) {

    const navigate = useNavigate();
    const [cookies] = useCookies(['user']);
    const [startIndex, setStartIndex] = useState(0);
    const [allQuestions, setAllQuestions] = useState(false)
    const [isDesktop, setDesktop] = useState(window.innerWidth > 1280);

    const updateMedia = () => {
        setDesktop(window.innerWidth > 1280);
    };

    useEffect(() => {
        window.addEventListener("resize", updateMedia);
        return () => window.removeEventListener("resize", updateMedia);
    });


    const fetchQuestions = async () => {
        try {
            console.log("hi")
            if (!allQuestions) {
                const res = await fetch(`/api/Questions?startIndex=${startIndex}`,
                    {
                        headers: {
                            'Content-Type': 'application/json',
                            'Accept': 'application/json'
                        }

                    });
                const data = await res.json();
                if (data.questions.length === 0) {
                    console.log("asdasdasd")
                    setAllQuestions(() => true)
                    return;
                }
                setNormalQuestion(() => [...normalQuestion, ...data.questions]);
                setsearchQuestion(() => [...searchQuestion, ...data.questions])
                setStartIndex(() => data.index)

            }
        } catch (error) {
            console.log(error);
        }
    };


    useEffect(() => {
        setNormalQuestion(() => []);
        setsearchQuestion(() => []);
        fetchQuestions();
    }, []);
    console.log(searchQuestion)
    return searchQuestion ? (
        <>
            <div className={"container flex justify-center items-center relative w-auto mx-auto " +(!isDesktop ? "flex-col gap-3" : "")}>
                <div
                    className="welcomePage text-center w-120 flex justify-center items-center flex-col mt-4" >
                    {searchQuestion.map((question) => {
                        return (<div
                            className='relative h-20 text-black border-b-2 border-l-2 rounded hover:bg-gray-100 transition w-120 overflow-hidden'
                        >
                            <h2 className='text-blue-700 cursor-pointer ml-2 break-words' onClick={() => {
                                navigate(`/question/${question.id}`);
                            }}>{question.title}</h2>
                            <p className='text-xs line-clamp-3 text-gray-600 leading-4 break-words ml-2'>{question.content}</p>
                            <div className={"flex flex-row gap-3 justify-center"}>
                            {question.tags.map((tag) => {
                                return (<Tag key={tag} tag={tag} />)
                            })}
                            </div>
                        </div>)
                    })}
                </div>
                <div
                    className={"trending w-80 top-0 right-0 flex justify-center items-center flex-col mt-4 border-l border-gray-200 " + (isDesktop ? "absolute" : "")}>
                    <p>
                        Trending Questions
                    </p>
                    <TrendingQuestions/>
                </div>
                </div>
                <IndexQuestion fetchMore={fetchQuestions}/>
            </>
        ) :
        <div>
            Loading
        </div>


    }
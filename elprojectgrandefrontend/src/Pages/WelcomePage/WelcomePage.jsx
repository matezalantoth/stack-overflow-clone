/* eslint-disable react/prop-types */
import {useEffect, useState} from "react";
import TrendingQuestions from "../../components/TrendingQuestions/TrendingQuestions.jsx";
import IndexQuestion from "../../components/IndexQuestion/IndexQuestion.jsx";
import {QuestionsComponent} from "./QuestionsComponent.jsx";


export default function WelcomePage({searchQuestion, normalQuestion, setNormalQuestion, setsearchQuestion}) {

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
            if (!allQuestions) {
                const res = await fetch(`/api/Questions?startIndex=${startIndex}`,
                    {
                        headers: {
                            'Content-Type': 'application/json',
                            'Accept': 'application/json'
                        }

                    });
                const data = await res.json();
                console.log(data);
                if (data.questions.length === 0) {
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
                <div
                    className={"container flex justify-center items-center relative w-auto mx-auto " + (!isDesktop ? "flex-col gap-3" : "")}>
                    <QuestionsComponent questions={searchQuestion}/>
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
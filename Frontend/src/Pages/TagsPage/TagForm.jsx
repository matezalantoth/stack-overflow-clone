import React, {useEffect, useState} from 'react';
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";

export default function TagForm({tag, setTag, setRenderForm}) {
    const [inputs, setInputs] = useState([]);
    const [cookies] = useCookies(['user']);

    useEffect(() => {
        setInputs({
            tagName: tag.tagName, description: tag.description
        });
    }, [])

    const updateTag = async () => {
        const res = await fetch("/api/Tags/" + tag.id, {
            method: "PUT",
            headers: {
                "Authorization": "Bearer " + cookies.user,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(inputs)
        })
        const data = await res.json();
        if (data.message) {
            toast.error(data.message)
            return;
        }
        setTag(prevTag => (
            {...prevTag, tagName: inputs.tagName, description: inputs.description}
        ));
    }

    return inputs ? (
        <div className="text-center">
            <form className="">

                <input className="text-2xl font-bold text-blue-500 mb-4 h-full w-full"
                       onChange={(e) => {
                           setInputs({...inputs, tagName: e.target.value})
                       }}
                       defaultValue={tag.tagName}/><br/>
                <div className="flex flex-col h-full">
                        <textarea
                            className="text-black w-full resize-none border-2 h-full flex-grow"
                            onChange={(e) => {
                                setInputs({...inputs, description: e.target.value});
                                e.target.style.height = 'auto';
                                e.target.style.height = `${e.target.scrollHeight}px`;
                            }}
                        >
        {tag.description}
    </textarea>
                </div>
                <button className="bg-red-500 rounded-md border-2 mr-2 mt-2 text-white border-red-500 px-2 mb-2"
                        onClick={(e) => {
                            e.preventDefault();
                            setRenderForm(false);
                        }}>Cancel
                </button>
                <button onClick={async (e) => {
                    e.preventDefault();
                    await updateTag();
                    setRenderForm(false);
                }} type="submit"
                        className="bg-green-400 text-white rounded-md border-2 border-green-400 px-2 mb-2">Save
                </button>

            </form>
        </div>
    ): (<></>)
}


/* eslint-disable react/prop-types */
import React, {useEffect, useState} from 'react';
import Tag from "../../components/Tag/Tag.jsx";
import {toast} from "react-hot-toast";
import {CheckIfSessionExpired} from "../../CheckIfSessionExpired.jsx";

export default function TagsPage({tags, setTags, setUserLoginCookies}) {
    const [isTagsLoaded, setIsTagsLoaded] = useState(false);
    const fetchTags = async () => {
        if (!isTagsLoaded) {
            const res = await fetch('api/Tags', {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                }
            })
            const data = await res.json();
            if (data.message) {
                toast.error(data.message);
                return;
            }
            setTags(data)
            setIsTagsLoaded(true);
        }
    }
    useEffect(() => {
        fetchTags();
    }, [])

    CheckIfSessionExpired(setUserLoginCookies);

    return (
        <div className>
            <div className={"text-center"}>
                <h1>Tags</h1>
                <label>Tags to categorize</label>
            </div>
            <div
                className={"p-4 top-1 grid 2xl:grid-cols-6 xl:grid-cols-5 lg:grid-cols-4 md:grid-cols-3 sm:grid-cols-2 gap-32 "}>
                {tags.map((tag) => (
                    <div className={"p-4 border border-black rounded h-44 w-44"} key={tag.id}>
                        <div>
                            <Tag tag={tag}/>
                        </div>
                        <label className={"text-xs text-gray-600 w-full"}>{tag.description}</label>
                    </div>
                ))}
            </div>
        </div>
    );
}


import React from 'react';

export default function Tag({tag}) {

    return (
        <div>
            <label className={"text-xs rounded bg-gray-300 p-1"}>{tag.tagName}</label>
        </div>
    );
}


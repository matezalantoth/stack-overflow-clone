import {useEffect, useRef, useState} from "react";

export default function IndexQuestion({fetchMore}) {
    const [isFetching, setIsFetching] = useState(false);
    const observer = useRef();

    useEffect(() => {
        const options = {
            root: null,
            rootMargin: "0px 0px 0px 0px",
            threshold: 1.0,
        };

        observer.current = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting && !isFetching) {
                    setIsFetching(true);
                    fetchMore().then(() => {
                        setIsFetching(false);
                    });
                }
            });
        }, options);

        if (observer.current) {
            observer.current.observe(document.getElementById("sentinel"));
        }

        return () => {
            if (observer.current) {
                observer.current.disconnect();
            }
        };
    }, [fetchMore, isFetching]);

    return (
        <div>

            <div id="sentinel"></div>
            {isFetching}
        </div>
    );
};
